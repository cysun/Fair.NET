ALTER SEQUENCE "Users_UserId_seq" RESTART WITH 10000;
ALTER SEQUENCE "Searches_SearchId_seq" RESTART WITH 10000;
ALTER SEQUENCE "Files_FileId_seq" RESTART WITH 1000000;
ALTER SEQUENCE "Documents_DocumentId_seq" RESTART WITH 1000000;
ALTER SEQUENCE "Comments_CommentId_seq" RESTART WITH 1000000;

INSERT INTO "Users" ("Username", "FirstName", "LastName", "Email", "IsAdmin", "IsSysAdmin") VALUES ('csun', 'Chengyu', 'Sun', 'csun@calstatela.edu', 't', 't');