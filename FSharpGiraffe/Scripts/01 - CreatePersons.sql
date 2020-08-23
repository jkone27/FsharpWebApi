IF OBJECT_ID(N'dbo.Persons', N'U') IS NULL
BEGIN

	CREATE TABLE dbo.Persons (
		Id INT PRIMARY KEY IDENTITY,
		[Name] NVARCHAR(20) NULL,
		Age INT NULL)

END

INSERT INTO dbo.Persons(Name, age)
VALUES('Giorgio',71),
('Pino',44)