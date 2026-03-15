-- =============================================
-- TABLE 2: DeliveryAdmins
-- =============================================
CREATE TABLE IF NOT EXISTS `DeliveryAdmins` (
    `DeliveryAdminId` INT NOT NULL AUTO_INCREMENT,
    `Username` VARCHAR(50) NOT NULL,
    `Password` VARCHAR(255) NOT NULL,
    `FullName` VARCHAR(100) NULL,
    `Email` VARCHAR(100) NULL,
    `Phone` VARCHAR(15) NULL,
    `Address` VARCHAR(500) NULL,
    `City` VARCHAR(100) NULL,
    `State` VARCHAR(100) NULL,
    `ProfileImage` VARCHAR(500) NULL,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `LastLoginDate` DATETIME NULL,
    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (`DeliveryAdminId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- INSERT: DeliveryAdmins
-- =============================================
INSERT INTO `DeliveryAdmins` (`Username`, `Password`, `FullName`, `Email`, `Phone`, `Address`, `City`, `State`, `IsActive`) VALUES
('deliveryadmin', 'admin123', 'Delivery Manager', 'delivery.admin@tastybites.com', '9876500001', '100 Ashram Road, Ahmedabad', 'Ahmedabad', 'Gujarat', 1),
('deliveryadmin2', 'admin123', 'Delivery Supervisor', 'delivery.supervisor@tastybites.com', '9876500002', '200 Paldi, Ahmedabad', 'Ahmedabad', 'Gujarat', 1);
