IF NOT EXISTS (
		SELECT *
		FROM Roles
		WHERE ROLE = 'Admin'
		)
BEGIN
	INSERT INTO Roles (ROLE, IsActive)
	VALUES ('Admin', 1)
END

IF NOT EXISTS (
		SELECT *
		FROM Roles
		WHERE ROLE = 'Admin'
		)
BEGIN
	INSERT INTO Roles (ROLE, IsActive)
	VALUES ('Client', 1)
END