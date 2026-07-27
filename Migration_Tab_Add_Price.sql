-- Lookup table for automatic party Default Rate Adjustment (rate_diff)
CREATE TABLE IF NOT EXISTS public.tab_add_price (
    rowid SERIAL PRIMARY KEY,
    company_id INT NULL,
    from_due NUMERIC(18, 2) NOT NULL,
    to_due NUMERIC(18, 2) NOT NULL,
    add_price NUMERIC(18, 2) NOT NULL,
    sale_per_comission NUMERIC(18, 2) NOT NULL DEFAULT 0
);

-- Ensure the column exists if the table was already created
ALTER TABLE public.tab_add_price ADD COLUMN IF NOT EXISTS sale_per_comission NUMERIC(18, 2) NOT NULL DEFAULT 0;

INSERT INTO public.tab_add_price (rowid, from_due, to_due, add_price, sale_per_comission)
VALUES
    (1, 0.00, 2.00, 0, 0),
    (2, 2.01, 3.00, 2, 0),
    (3, 3.01, 4.00, 4, 0),
    (4, 4.01, 5.00, 5, 0),
    (5, 5.01, 6.00, 7, 0),
    (6, 6.01, 7.00, 8, 0),
    (7, 7.01, 8.00, 10, 0),
    (8, 8.01, 9.00, 12, 0),
    (9, 9.01, 10.00, 14, 0),
    (10, 10.01, 20.00, 29, 0),
    (11, 20.01, 1000.00, 1000, 0)
ON CONFLICT (rowid) DO UPDATE SET
    from_due = EXCLUDED.from_due,
    to_due = EXCLUDED.to_due,
    add_price = EXCLUDED.add_price,
    sale_per_comission = EXCLUDED.sale_per_comission;
