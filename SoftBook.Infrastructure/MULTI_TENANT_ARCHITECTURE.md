# Multi-Tenant Architecture Guide for SoftAccDb

## Overview

This document explains the multi-tenant architecture implemented for SoftAccDb PostgreSQL migration, where **CompanyId** is used to filter data across all tables.

---

## Architecture Approach: **Database Sharding + Row-Level Filtering**

### Your Current Design

```
┌─────────────────────────────────────────────────────────┐
│  CompanyMaster Database (PostgreSQL)                     │
│  ┌───────────────────────────────────────────────────┐   │
│  │ tab_company_master                                │   │
│  │ - company_id (1-10000) → ConnectionString DB1   │   │
│  │ - company_id (10001-20000) → ConnectionString DB2│   │
│  └───────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                          │
                          │ Routes to
                          ▼
        ┌─────────────────────────────────────┐
        │  SoftAccDb Database 1 (PostgreSQL)  │
        │  Companies: 1-10000                 │
        │  ┌───────────────────────────────┐  │
        │  │ All tables have company_id     │  │
        │  │ Filter: WHERE company_id = ?  │  │
        │  └───────────────────────────────┘  │
        └─────────────────────────────────────┘
                          │
        ┌─────────────────────────────────────┐
        │  SoftAccDb Database 2 (PostgreSQL)  │
        │  Companies: 10001-20000             │
        │  ┌───────────────────────────────┐  │
        │  │ All tables have company_id     │  │
        │  │ Filter: WHERE company_id = ?  │  │
        │  └───────────────────────────────┘  │
        └─────────────────────────────────────┘
```

---

## ✅ **Is This Approach Correct?**

### **YES, with some considerations:**

#### **Advantages:**
1. ✅ **Cost-Effective**: Share database resources across 10,000 companies
2. ✅ **Scalability**: Easy to add new databases when one fills up
3. ✅ **Isolation**: Each database shard is independent
4. ✅ **Performance**: Smaller databases = faster queries
5. ✅ **Backup/Restore**: Can backup individual shards

#### **Important Considerations:**

### 1. **CompanyId Filtering is CRITICAL**
   - **Every query MUST include `WHERE company_id = ?`**
   - **Never trust application code alone** - use database-level security

### 2. **Connection String Routing**
   ```csharp
   // Pseudo-code for your application
   public string GetConnectionString(int companyId)
   {
       // Query CompanyMaster database
       var company = GetCompanyFromMaster(companyId);
       
       // Route based on company_id range
       if (companyId >= 1 && companyId <= 10000)
           return "ConnectionString_DB1";
       else if (companyId >= 10001 && companyId <= 20000)
           return "ConnectionString_DB2";
       // ... etc
   }
   ```

### 3. **Row-Level Security (RLS) - RECOMMENDED**
   PostgreSQL supports **Row-Level Security** policies that **automatically** filter by CompanyId:

   ```sql
   -- Enable RLS on all tables
   ALTER TABLE tab_account_master ENABLE ROW LEVEL SECURITY;
   
   -- Create policy
   CREATE POLICY company_isolation_policy ON tab_account_master
       FOR ALL
       USING (company_id = current_setting('app.company_id')::INT);
   ```

   **Benefits:**
   - **Automatic filtering** - Even if developer forgets WHERE clause
   - **Security** - Prevents data leakage between companies
   - **Transparent** - Works with all queries automatically

---

## Implementation Steps

### Step 1: **CompanyMaster Database Setup**
   - Already created: `tab_company_master` table
   - Stores routing information (company_id → connection_string)

### Step 2: **SoftAccDb Database Setup**
   - Run `SoftAccDB_PostgreSQL.sql` to create all tables
   - All tables have `company_id INT NOT NULL` column
   - All tables have indexes on `company_id` for performance

### Step 3: **Application Code Changes**
   ```csharp
   // Example: Your repository should ALWAYS filter by CompanyId
   public async Task<List<Account>> GetAccountsAsync(int companyId)
   {
       return await _dbContext.Accounts
           .Where(a => a.CompanyId == companyId)  // REQUIRED!
           .ToListAsync();
   }
   ```

### Step 4: **Enable Row-Level Security (Optional but Recommended)**
   ```sql
   -- Run this script on each SoftAccDb database
   -- This ensures data isolation even if application code has bugs
   
   ALTER TABLE tab_account_master ENABLE ROW LEVEL SECURITY;
   CREATE POLICY account_company_isolation ON tab_account_master
       FOR ALL USING (company_id = current_setting('app.company_id', true)::INT);
   
   -- Repeat for ALL tables...
   ```

---

## Best Practices

### ✅ **DO:**
1. ✅ **Always include CompanyId in WHERE clauses**
2. ✅ **Use indexes** (already created in migration script)
3. ✅ **Enable Row-Level Security** for extra protection
4. ✅ **Monitor database size** - plan for sharding when approaching limits
5. ✅ **Use connection pooling** per database shard
6. ✅ **Log all queries** to detect missing CompanyId filters

### ❌ **DON'T:**
1. ❌ **Never query without CompanyId filter**
2. ❌ **Don't use global admin queries** (unless absolutely necessary)
3. ❌ **Don't hardcode company_id ranges** - use CompanyMaster lookup
4. ❌ **Don't forget foreign keys** - they ensure data integrity

---

## Migration Strategy

### **From SQL Server to PostgreSQL:**

1. **Export data from SQL Server** (per company or all)
2. **Import to PostgreSQL** with CompanyId populated
3. **Verify row counts** match
4. **Test queries** with CompanyId filtering
5. **Enable RLS** after verification

### **Data Migration Script Example:**
```sql
-- When migrating existing SQL Server data
-- You'll need to assign CompanyId to each row

-- Example: If migrating company_id = 1
INSERT INTO tab_account_master (company_id, account_type, account_name, ...)
SELECT 1, account_type, account_name, ... 
FROM sql_server_source_table;
```

---

## Performance Considerations

### **Indexes Created:**
- ✅ Every table has `idx_<table>_company` index on `company_id`
- ✅ Composite indexes on frequently queried columns + company_id

### **Query Performance:**
```sql
-- FAST: Uses index on company_id
SELECT * FROM tab_account_master WHERE company_id = 123;

-- SLOW: Full table scan (if no other index)
SELECT * FROM tab_account_master WHERE account_name = 'Cash';
```

### **Recommended Composite Indexes:**
```sql
-- For frequently queried combinations
CREATE INDEX idx_account_company_type ON tab_account_master(company_id, account_type);
CREATE INDEX idx_party_company_type ON tab_party_master(company_id, party_type);
```

---

## Monitoring & Maintenance

### **Key Metrics to Monitor:**
1. **Database size per shard**
2. **Query performance** (especially with CompanyId filter)
3. **Connection pool usage**
4. **Row counts per company** (detect outliers)

### **When to Create New Shard:**
- When database size approaches 80% of limit
- When query performance degrades
- When you reach company_id range limit (e.g., 10000)

---

## Security Considerations

### **1. Application-Level:**
- ✅ Always validate CompanyId from JWT/Claims
- ✅ Use middleware to inject CompanyId into queries
- ✅ Never trust client-provided CompanyId without validation

### **2. Database-Level:**
- ✅ Use Row-Level Security (RLS) policies
- ✅ Use database roles with limited permissions
- ✅ Audit logs for all queries

---

## Summary

Your approach of **sharding by company_id ranges (1-10000, 10001-20000)** combined with **CompanyId filtering in all tables** is:

✅ **Correct and Scalable**
✅ **Cost-Effective**
✅ **Industry Standard** (used by Salesforce, Shopify, etc.)

**Key Success Factors:**
1. ✅ Always filter by CompanyId
2. ✅ Use Row-Level Security for extra protection
3. ✅ Monitor database sizes and plan sharding ahead
4. ✅ Test thoroughly before production

---

## Next Steps

1. ✅ Run `SoftAccDB_PostgreSQL.sql` to create schema
2. ✅ Migrate data from SQL Server with CompanyId
3. ✅ Enable Row-Level Security (optional but recommended)
4. ✅ Update application code to always filter by CompanyId
5. ✅ Test with multiple companies in same database

---

## Questions?

If you need help with:
- Row-Level Security setup
- Data migration scripts
- Application code changes
- Performance optimization

Let me know!


