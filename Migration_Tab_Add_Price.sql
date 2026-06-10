-- Lookup table for automatic party Default Rate Adjustment (rate_diff)
CREATE TABLE IF NOT EXISTS public.tab_add_price (
    rowid SERIAL PRIMARY KEY,
    company_id INT NULL,
    from_due NUMERIC(18, 2) NOT NULL,
    to_due NUMERIC(18, 2) NOT NULL,
    add_price NUMERIC(18, 2) NOT NULL
);

INSERT INTO public.tab_add_price (rowid, from_due, to_due, add_price)
VALUES
    (1, 0.00, 2.00, 0),
    (2, 2.01, 3.00, 2),
    (3, 3.01, 4.00, 4),
    (4, 4.01, 5.00, 5),
    (5, 5.01, 6.00, 7),
    (6, 6.01, 7.00, 8),
    (7, 7.01, 8.00, 10),
    (8, 8.01, 9.00, 12),
    (9, 9.01, 10.00, 14),
    (10, 10.01, 20.00, 29),
    (11, 20.01, 1000.00, 1000)
ON CONFLICT (rowid) DO UPDATE SET
    from_due = EXCLUDED.from_due,
    to_due = EXCLUDED.to_due,
    add_price = EXCLUDED.add_price;
