CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE "Files" (
    "FileId" serial NOT NULL,
    "Name" text NULL,
    "ContentType" text NULL,
    "Timestamp" timestamp without time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "Content" bytea NULL,
    CONSTRAINT "PK_Files" PRIMARY KEY ("FileId")
);

CREATE TABLE "Users" (
    "UserId" serial NOT NULL,
    "Username" character varying(255) NOT NULL,
    "FirstName" character varying(255) NOT NULL,
    "LastName" character varying(255) NOT NULL,
    "Email" character varying(255) NOT NULL,
    "IsAdmin" boolean NOT NULL DEFAULT FALSE,
    CONSTRAINT "PK_Users" PRIMARY KEY ("UserId"),
    CONSTRAINT "AK_Users_Username" UNIQUE ("Username")
);

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20190827203111_InitialSchema', '2.2.4-servicing-10062');

