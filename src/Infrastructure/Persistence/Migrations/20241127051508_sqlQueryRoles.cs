using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAPI.Infrastructure.Migrations
{
    public partial class sqlQueryRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.Sql("INSERT INTO [dbo].[AspNetUserRoles] ( UserId,RoleId ) SELECT  (SELECT top 1 UserId  FROM AspNetUsers) as UserId, (SELECT top 1 Id  FROM AspNetRoles) as RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
