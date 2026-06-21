using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hydash.WebApp.Migrations
{
    /// <inheritdoc />
    public partial class PermissionAndRolePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS `Permissions` (
                    `Id` int NOT NULL AUTO_INCREMENT,
                    `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
                    `Description` longtext CHARACTER SET utf8mb4 NOT NULL,
                    CONSTRAINT `PK_Permissions` PRIMARY KEY (`Id`)
                ) CHARACTER SET=utf8mb4;
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS `RolePermissions` (
                    `RolePermissionId` int NOT NULL AUTO_INCREMENT,
                    `RoleId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
                    `PermissionId` int NOT NULL,
                    CONSTRAINT `PK_RolePermissions` PRIMARY KEY (`RolePermissionId`),
                    CONSTRAINT `FK_RolePermissions_AspNetRoles_RoleId`
                        FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
                    CONSTRAINT `FK_RolePermissions_Permissions_PermissionId`
                        FOREIGN KEY (`PermissionId`) REFERENCES `Permissions` (`Id`) ON DELETE CASCADE
                ) CHARACTER SET=utf8mb4;
            ");

            // Conditionally create indexes (MySQL has no CREATE INDEX IF NOT EXISTS)
            migrationBuilder.Sql(@"
                DROP PROCEDURE IF EXISTS `hydash_create_indexes`;
                CREATE PROCEDURE `hydash_create_indexes`()
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.statistics
                        WHERE table_schema = DATABASE()
                          AND table_name = 'RolePermissions'
                          AND index_name = 'IX_RolePermissions_PermissionId'
                    ) THEN
                        CREATE INDEX `IX_RolePermissions_PermissionId` ON `RolePermissions` (`PermissionId`);
                    END IF;

                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.statistics
                        WHERE table_schema = DATABASE()
                          AND table_name = 'RolePermissions'
                          AND index_name = 'IX_RolePermissions_RoleId'
                    ) THEN
                        CREATE INDEX `IX_RolePermissions_RoleId` ON `RolePermissions` (`RoleId`);
                    END IF;
                END;
                CALL `hydash_create_indexes`();
                DROP PROCEDURE IF EXISTS `hydash_create_indexes`;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "RolePermissions");
            migrationBuilder.DropTable(name: "Permissions");
        }
    }
}