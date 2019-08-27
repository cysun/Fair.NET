ALTER SEQUENCE "Users_UserId_seq" RESTART WITH 1000;
ALTER SEQUENCE "Files_FileId_seq" RESTART WITH 1000000;

INSERT INTO "Users" ("Username", "FirstName", "LastName", "Email", "IsAdmin") VALUES ('csun', 'Chengyu', 'Sun', 'csun@calstatela.edu', 't');