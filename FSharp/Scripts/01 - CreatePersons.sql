CREATE SCHEMA IF NOT EXISTS dbo;

CREATE TABLE IF NOT EXISTS dbo.persons (
    id SERIAL PRIMARY KEY,
    name VARCHAR(20) NULL,
    age INT NULL
);

INSERT INTO dbo.persons(name, age)
VALUES
    ('Giorgio', 71),
    ('Pino', 44),
    ('Luigi', 35),
    ('Maria', 28),
    ('Anna', 22)
ON CONFLICT DO NOTHING;