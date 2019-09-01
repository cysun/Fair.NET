CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE "Users" (
    "UserId" serial NOT NULL,
    "Username" character varying(255) NOT NULL,
    "FirstName" character varying(255) NOT NULL,
    "LastName" character varying(255) NOT NULL,
    "Email" character varying(255) NOT NULL,
    "IsAdmin" boolean NOT NULL DEFAULT FALSE,
    "IsSysAdmin" boolean NOT NULL DEFAULT FALSE,
    CONSTRAINT "PK_Users" PRIMARY KEY ("UserId"),
    CONSTRAINT "AK_Users_Username" UNIQUE ("Username")
);

CREATE TABLE "Comments" (
    "CommentId" serial NOT NULL,
    "AuthorId" integer NOT NULL,
    "Content" text NULL,
    "Timestamp" timestamp without time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "PK_Comments" PRIMARY KEY ("CommentId"),
    CONSTRAINT "FK_Comments_Users_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES "Users" ("UserId") ON DELETE CASCADE
);

CREATE TABLE "Files" (
    "FileId" serial NOT NULL,
    "Name" text NULL,
    "ContentType" text NULL,
    "Timestamp" timestamp without time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "OwnerId" integer NOT NULL,
    "Content" bytea NULL,
    CONSTRAINT "PK_Files" PRIMARY KEY ("FileId"),
    CONSTRAINT "FK_Files_Users_OwnerId" FOREIGN KEY ("OwnerId") REFERENCES "Users" ("UserId") ON DELETE CASCADE
);

CREATE TABLE "Searches" (
    "SearchId" serial NOT NULL,
    "Name" character varying(255) NOT NULL,
    "StartDate" timestamp without time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "CloseDate" timestamp without time zone NULL,
    "DepartmentChairId" integer NOT NULL,
    "CommitteeChairId" integer NOT NULL,
    CONSTRAINT "PK_Searches" PRIMARY KEY ("SearchId"),
    CONSTRAINT "FK_Searches_Users_CommitteeChairId" FOREIGN KEY ("CommitteeChairId") REFERENCES "Users" ("UserId") ON DELETE CASCADE,
    CONSTRAINT "FK_Searches_Users_DepartmentChairId" FOREIGN KEY ("DepartmentChairId") REFERENCES "Users" ("UserId") ON DELETE CASCADE
);

CREATE TABLE "CommitteeMembers" (
    "SearchId" integer NOT NULL,
    "UserId" integer NOT NULL,
    CONSTRAINT "PK_CommitteeMembers" PRIMARY KEY ("SearchId", "UserId"),
    CONSTRAINT "FK_CommitteeMembers_Searches_SearchId" FOREIGN KEY ("SearchId") REFERENCES "Searches" ("SearchId") ON DELETE CASCADE,
    CONSTRAINT "FK_CommitteeMembers_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE CASCADE
);

CREATE TABLE "Documents" (
    "DocumentId" serial NOT NULL,
    "SearchId" integer NOT NULL,
    "Name" character varying(255) NOT NULL,
    CONSTRAINT "PK_Documents" PRIMARY KEY ("DocumentId"),
    CONSTRAINT "FK_Documents_Searches_SearchId" FOREIGN KEY ("SearchId") REFERENCES "Searches" ("SearchId") ON DELETE CASCADE
);

CREATE TABLE "Revisions" (
    "DocumentId" integer NOT NULL,
    "Number" integer NOT NULL,
    "AuthorId" integer NOT NULL,
    "CommentId" integer NULL,
    "FileId" integer NOT NULL,
    CONSTRAINT "PK_Revisions" PRIMARY KEY ("DocumentId", "Number"),
    CONSTRAINT "FK_Revisions_Users_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES "Users" ("UserId") ON DELETE CASCADE,
    CONSTRAINT "FK_Revisions_Comments_CommentId" FOREIGN KEY ("CommentId") REFERENCES "Comments" ("CommentId") ON DELETE RESTRICT,
    CONSTRAINT "FK_Revisions_Documents_DocumentId" FOREIGN KEY ("DocumentId") REFERENCES "Documents" ("DocumentId") ON DELETE CASCADE,
    CONSTRAINT "FK_Revisions_Files_FileId" FOREIGN KEY ("FileId") REFERENCES "Files" ("FileId") ON DELETE CASCADE
);

CREATE INDEX "IX_Comments_AuthorId" ON "Comments" ("AuthorId");

CREATE INDEX "IX_CommitteeMembers_UserId" ON "CommitteeMembers" ("UserId");

CREATE INDEX "IX_Documents_SearchId" ON "Documents" ("SearchId");

CREATE INDEX "IX_Files_OwnerId" ON "Files" ("OwnerId");

CREATE INDEX "IX_Revisions_AuthorId" ON "Revisions" ("AuthorId");

CREATE INDEX "IX_Revisions_CommentId" ON "Revisions" ("CommentId");

CREATE INDEX "IX_Revisions_FileId" ON "Revisions" ("FileId");

CREATE INDEX "IX_Searches_CommitteeChairId" ON "Searches" ("CommitteeChairId");

CREATE INDEX "IX_Searches_DepartmentChairId" ON "Searches" ("DepartmentChairId");

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20190901062335_InitialSchema', '2.2.4-servicing-10062');

