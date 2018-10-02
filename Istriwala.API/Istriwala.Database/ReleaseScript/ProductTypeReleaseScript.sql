IF NOT EXISTS ( SELECT * FROM Roles WHERE ROLE = 'Male' )
BEGIN  insert into ProductType(ProductType) values ('Male') END

IF NOT EXISTS ( SELECT * FROM Roles WHERE ROLE = 'Female' )
BEGIN  insert into ProductType(ProductType) values ('Female') END
