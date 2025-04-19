DO
$do$
    BEGIN

        CREATE TABLE IF NOT EXISTS orders
        (
            id               UUID PRIMARY KEY,
            product_name     TEXT        NOT NULL,
            delivery_address TEXT        NOT NULL,
--             status           TEXT        NOT NULL, -- No way to store enums as strings wtf dapper
            status           INTEGER     NOT NULL,
            is_archived      BOOLEAN     NOT NULL,
            created_at       TIMESTAMPTZ NOT NULL,
            last_modified_at TIMESTAMPTZ
        );

        --         IF NOT EXISTS (SELECT 1
--                        FROM pg_constraint c
--                        JOIN pg_class t ON c.conrelid = t.oid
--                        JOIN pg_namespace n ON t.relnamespace = n.oid
--                        WHERE c.conname = 'check_status_values'
--                          AND t.relname = 'orders'
--                          AND n.nspname = CURRENT_SCHEMA()) THEN
--             ALTER TABLE orders
--                 ADD CONSTRAINT check_status_values
--                     CHECK (status IN ('New', 'Dispatched', 'OutForDelivery', 'Delivered'));
--         END IF;

        CREATE INDEX IF NOT EXISTS idx_orders_status ON orders (status);

        CREATE INDEX IF NOT EXISTS idx_orders_created_at ON orders (created_at);
    END
$do$;