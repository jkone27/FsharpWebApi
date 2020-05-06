CREATE TABLE dbo.Persons (
Id INT PRIMARY KEY IDENTITY,
[Name] NVARCHAR(20) NULL,
Age INT NULL)

INSERT INTO dbo.Persons(Name, age)
VALUES('Giorgio',71),
('Pino',44)

SELECT * FROM dbo.Persons