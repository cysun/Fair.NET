ALTER TABLE IF EXISTS "Documents" DROP CONSTRAINT "FK_Documents_Revisions_LatestRevisionId";
DROP TABLE IF EXISTS "Revisions";
DROP TABLE IF EXISTS "Files";
DROP TABLE IF EXISTS "Documents";
DROP TABLE IF EXISTS "CommitteeMembers";
DROP TABLE IF EXISTS "Searches";
DROP TABLE IF EXISTS "Users";
DROP TABLE IF EXISTS "__EFMigrationsHistory";