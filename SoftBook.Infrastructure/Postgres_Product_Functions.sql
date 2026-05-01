-- Product Create
CREATE OR REPLACE FUNCTION sp_product_create(
    p_company_id INT,
    p_product_type VARCHAR,
    p_product_name VARCHAR,
    p_short_name VARCHAR,
    p_selling_price NUMERIC,
    p_sales_account VARCHAR,
    p_sales_description VARCHAR,
    p_cost_price NUMERIC,
    p_purchase_account VARCHAR,
    p_purchase_description VARCHAR,
    p_preferred_vendor_id INT,
    p_sellable BOOLEAN,
    p_purchasable BOOLEAN,
    p_unit_id INT,
    p_hsn_id INT,
    p_sac_id INT,
    p_tax_preference_id INT,
    p_tax_group_id INT,
    p_product_group_id INT,
    p_product_packing_unit INT,
    p_opening_quantity NUMERIC,
    p_opening_price NUMERIC,
    p_opening_amount NUMERIC,
    p_purchase_effected_account INT,
    p_sale_effected_account INT,
    p_selling_price_2 NUMERIC,
    p_selling_price_3 NUMERIC,
    p_selling_price_4 NUMERIC,
    p_selling_price_5 NUMERIC,
    p_min_quantity NUMERIC,
    p_max_quantity NUMERIC,
    p_re_order_quantity NUMERIC,
    p_tax_exemption_id INT,
    p_user_id INT DEFAULT NULL
)
RETURNS VOID AS $$
BEGIN
    -- Authorization Guard
    PERFORM public.fn_validate_permission(p_user_id, p_company_id, 11, 'ADD');

    INSERT INTO tab_product_master (
        company_id, product_type, product_name, short_name, selling_price, sales_account, sales_description,
        cost_price, purchase_account, purchase_description, preferred_vendor_id,
        sellable, purchasable, unit_id, hsn_id, sac_id, tax_preference_id, tax_group_id,
        product_group_id, product_packing_unit, opening_quantity, opening_price, opening_amount,
        purchase_effected_account, sale_effected_account, selling_price_2, selling_price_3,
        selling_price_4, selling_price_5, min_quantity, max_quantity, re_order_quantity, tax_exemption_id
    )
    VALUES (
        p_company_id, p_product_type, p_product_name, p_short_name, p_selling_price, p_sales_account, p_sales_description,
        p_cost_price, p_purchase_account, p_purchase_description, p_preferred_vendor_id,
        COALESCE(p_sellable, TRUE), COALESCE(p_purchasable, TRUE), p_unit_id, p_hsn_id, p_sac_id, p_tax_preference_id, p_tax_group_id,
        p_product_group_id, p_product_packing_unit, p_opening_quantity, p_opening_price, p_opening_amount,
        p_purchase_effected_account, p_sale_effected_account, p_selling_price_2, p_selling_price_3,
        p_selling_price_4, p_selling_price_5, p_min_quantity, p_max_quantity, p_re_order_quantity, p_tax_exemption_id
    );
END;
$$ LANGUAGE plpgsql;

-- Product Update
CREATE OR REPLACE FUNCTION sp_product_update(
    p_product_id INT,
    p_company_id INT,
    p_product_type VARCHAR,
    p_product_name VARCHAR,
    p_short_name VARCHAR,
    p_selling_price NUMERIC,
    p_sales_account VARCHAR,
    p_sales_description VARCHAR,
    p_cost_price NUMERIC,
    p_purchase_account VARCHAR,
    p_purchase_description VARCHAR,
    p_preferred_vendor_id INT,
    p_sellable BOOLEAN,
    p_purchasable BOOLEAN,
    p_unit_id INT,
    p_hsn_id INT,
    p_sac_id INT,
    p_tax_preference_id INT,
    p_tax_group_id INT,
    p_product_group_id INT,
    p_product_packing_unit INT,
    p_opening_quantity NUMERIC,
    p_opening_price NUMERIC,
    p_opening_amount NUMERIC,
    p_purchase_effected_account INT,
    p_sale_effected_account INT,
    p_selling_price_2 NUMERIC,
    p_selling_price_3 NUMERIC,
    p_selling_price_4 NUMERIC,
    p_selling_price_5 NUMERIC,
    p_min_quantity NUMERIC,
    p_max_quantity NUMERIC,
    p_re_order_quantity NUMERIC,
    p_tax_exemption_id INT,
    p_user_id INT DEFAULT NULL
)
RETURNS VOID AS $$
BEGIN
    -- Authorization Guard
    PERFORM public.fn_validate_permission(p_user_id, p_company_id, 11, 'EDIT');

    UPDATE tab_product_master
    SET
        product_type = p_product_type,
        product_name = p_product_name,
        short_name = p_short_name,
        selling_price = p_selling_price,
        sales_account = p_sales_account,
        sales_description = p_sales_description,
        cost_price = p_cost_price,
        purchase_account = p_purchase_account,
        purchase_description = p_purchase_description,
        preferred_vendor_id = p_preferred_vendor_id,
        sellable = p_sellable,
        purchasable = p_purchasable,
        unit_id = p_unit_id,
        hsn_id = p_hsn_id,
        sac_id = p_sac_id,
        tax_preference_id = p_tax_preference_id,
        tax_group_id = p_tax_group_id,
        product_group_id = p_product_group_id,
        product_packing_unit = p_product_packing_unit,
        opening_quantity = p_opening_quantity,
        opening_price = p_opening_price,
        opening_amount = p_opening_amount,
        purchase_effected_account = p_purchase_effected_account,
        sale_effected_account = p_sale_effected_account,
        selling_price_2 = p_selling_price_2,
        selling_price_3 = p_selling_price_3,
        selling_price_4 = p_selling_price_4,
        selling_price_5 = p_selling_price_5,
        min_quantity = p_min_quantity,
        max_quantity = p_max_quantity,
        re_order_quantity = p_re_order_quantity,
        tax_exemption_id = p_tax_exemption_id
    WHERE product_id = p_product_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

-- Product Delete
CREATE OR REPLACE FUNCTION sp_product_delete(
    p_product_id INT,
    p_company_id INT,
    p_user_id INT DEFAULT NULL
)
RETURNS VOID AS $$
BEGIN
    -- Authorization Guard
    PERFORM public.fn_validate_permission(p_user_id, p_company_id, 11, 'DELETE');

    DELETE FROM tab_product_master
    WHERE product_id = p_product_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

-- Get All Products (Basic)
CREATE OR REPLACE FUNCTION sp_product_get_all(
    p_company_id INT
)
RETURNS SETOF tab_product_master AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM tab_product_master
    WHERE company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

-- Get Product By ID
CREATE OR REPLACE FUNCTION sp_product_get_by_id(
    p_product_id INT,
    p_company_id INT
)
RETURNS SETOF tab_product_master AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM tab_product_master
    WHERE product_id = p_product_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

-- Get All Products with Details (Joins)
DROP FUNCTION IF EXISTS public.sp_product_get_all_with_details(integer);

CREATE OR REPLACE FUNCTION public.sp_product_get_all_with_details(
	p_company_id integer)
    RETURNS TABLE(
        product_id integer, 
        product_name character varying, 
        short_name character varying,
        product_type character varying, 
        mrp numeric,
        selling_price numeric, 
        sales_description text, 
        purchase_description text, 
        cost_price numeric, 
        tax_group_id integer, 
        tax_preference_id integer, 
        base_unit character varying, 
        packing_unit character varying, 
        product_group_name character varying, 
        hsn_code character varying, 
        sac_code character varying, 
        tax_group_name character varying, 
        min_quantity numeric, 
        max_quantity numeric, 
        re_order_quantity numeric,
        cgst_per numeric,
        sgst_per numeric,
        unit_id integer,
        product_packing_unit integer
    ) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT
        P.product_id,
        P.product_name,
        P.short_name,
        P.product_type,
        P.mrp,
        P.selling_price,
        P.sales_description,
        P.purchase_description,
        P.cost_price,

        P.tax_group_id,
        P.tax_preference_id,

        U.unit_name AS base_unit,
        PU.unit_name AS packing_unit,

        PG.product_group_name,

        H.hsn_code,
        S.sac_code,

        TG.group_name AS tax_group_name,

        P.min_quantity,
        P.max_quantity,
        P.re_order_quantity,

        TG.cgst_per,
        TG.sgst_per,
        P.unit_id,
        P.product_packing_unit

    FROM tab_product_master P

    LEFT JOIN tab_unit_master U
        ON P.unit_id = U.unit_id

    LEFT JOIN tab_unit_master PU
        ON P.product_packing_unit = PU.unit_id

    LEFT JOIN tab_product_group_master PG
        ON P.product_group_id = PG.product_group_id

    LEFT JOIN tab_hsn_master H
        ON P.hsn_id = H.hsn_id

    LEFT JOIN tab_sac_master S
        ON P.sac_id = S.sac_id

    LEFT JOIN tab_tax_group_master TG
        ON P.tax_group_id = TG.tax_group_id

    WHERE P.company_id = p_company_id

    ORDER BY P.product_id DESC;
END;
$BODY$;

ALTER FUNCTION public.sp_product_get_all_with_details(integer)
OWNER TO postgres;

-- Unified Product Function for Repository
CREATE OR REPLACE FUNCTION fn_product_master(
    p_action VARCHAR,
    p_company_id INT,
    p_product_id INT DEFAULT NULL,
    p_product_type VARCHAR DEFAULT NULL,
    p_product_name VARCHAR DEFAULT NULL,
    p_short_name VARCHAR DEFAULT NULL,
    p_selling_price NUMERIC DEFAULT NULL,
    p_sales_account VARCHAR DEFAULT NULL,
    p_sales_description VARCHAR DEFAULT NULL,
    p_cost_price NUMERIC DEFAULT NULL,
    p_purchase_account VARCHAR DEFAULT NULL,
    p_purchase_description VARCHAR DEFAULT NULL,
    p_preferred_vendor_id INT DEFAULT NULL,
    p_sellable BOOLEAN DEFAULT NULL,
    p_purchasable BOOLEAN DEFAULT NULL,
    p_unit_id INT DEFAULT NULL,
    p_hsn_id INT DEFAULT NULL,
    p_sac_id INT DEFAULT NULL,
    p_tax_preference_id INT DEFAULT NULL,
    p_tax_group_id INT DEFAULT NULL,
    p_product_group_id INT DEFAULT NULL,
    p_product_packing_unit INT DEFAULT NULL,
    p_opening_quantity NUMERIC DEFAULT NULL,
    p_opening_price NUMERIC DEFAULT NULL,
    p_opening_amount NUMERIC DEFAULT NULL,
    p_purchase_effected_account INT DEFAULT NULL,
    p_sale_effected_account INT DEFAULT NULL,
    p_selling_price_2 NUMERIC DEFAULT NULL,
    p_selling_price_3 NUMERIC DEFAULT NULL,
    p_selling_price_4 NUMERIC DEFAULT NULL,
    p_selling_price_5 NUMERIC DEFAULT NULL,
    p_min_quantity NUMERIC DEFAULT NULL,
    p_max_quantity NUMERIC DEFAULT NULL,
    p_re_order_quantity NUMERIC DEFAULT NULL,
    p_tax_exemption_id INT DEFAULT NULL
)
RETURNS SETOF tab_product_master AS $$
BEGIN
    IF p_action = 'GETALL' THEN
        RETURN QUERY SELECT * FROM sp_product_get_all(p_company_id);
    ELSIF p_action = 'GETBYID' THEN
        RETURN QUERY SELECT * FROM sp_product_get_by_id(p_product_id, p_company_id);
    ELSIF p_action = 'INSERT' THEN
        PERFORM sp_product_create(
            p_company_id, p_product_type, p_product_name, p_short_name, p_selling_price, p_sales_account, p_sales_description,
            p_cost_price, p_purchase_account, p_purchase_description, p_preferred_vendor_id,
            p_sellable, p_purchasable, p_unit_id, p_hsn_id, p_sac_id, p_tax_preference_id, p_tax_group_id,
            p_product_group_id, p_product_packing_unit, p_opening_quantity, p_opening_price, p_opening_amount,
            p_purchase_effected_account, p_sale_effected_account, p_selling_price_2, p_selling_price_3,
            p_selling_price_4, p_selling_price_5, p_min_quantity, p_max_quantity, p_re_order_quantity, p_tax_exemption_id
        );
        RETURN;
    ELSIF p_action = 'UPDATE' THEN
        PERFORM sp_product_update(
            p_product_id, p_company_id, p_product_type, p_product_name, p_short_name, p_selling_price, p_sales_account, p_sales_description,
            p_cost_price, p_purchase_account, p_purchase_description, p_preferred_vendor_id,
            p_sellable, p_purchasable, p_unit_id, p_hsn_id, p_sac_id, p_tax_preference_id, p_tax_group_id,
            p_product_group_id, p_product_packing_unit, p_opening_quantity, p_opening_price, p_opening_amount,
            p_purchase_effected_account, p_sale_effected_account, p_selling_price_2, p_selling_price_3,
            p_selling_price_4, p_selling_price_5, p_min_quantity, p_max_quantity, p_re_order_quantity, p_tax_exemption_id
        );
        RETURN;
    ELSIF p_action = 'DELETE' THEN
        PERFORM sp_product_delete(p_product_id, p_company_id);
        RETURN;
    END IF;
END;
$$ LANGUAGE plpgsql;
