ALTER SEQUENCE "Users_Id_seq" RESTART WITH 10000;
ALTER SEQUENCE "Departments_Id_seq" RESTART WITH 1000;
ALTER SEQUENCE "Searches_Id_seq" RESTART WITH 10000;
ALTER SEQUENCE "Files_Id_seq" RESTART WITH 1000000;
ALTER SEQUENCE "Revisions_Id_seq" RESTART WITH 1000000;
ALTER SEQUENCE "Documents_Id_seq" RESTART WITH 1000000;

INSERT INTO "Users" ("Username", "FirstName", "LastName", "Email", "IsAdmin", "IsSysAdmin") VALUES ('csun', 'Chengyu', 'Sun', 'csun@calstatela.edu', 't', 't');
INSERT INTO "Users" ("Username", "FirstName", "LastName", "Email") VALUES ('eykang', 'Elaine', 'Kang', 'eykang@calstatela.edu');
INSERT INTO "Users" ("Username", "FirstName", "LastName", "Email") VALUES ('rpamula', 'Raj', 'Pamula', 'rpamula@calstatela.edu');
INSERT INTO "Users" ("Username", "FirstName", "LastName", "Email") VALUES ('rabbott', 'Russ', 'Abbott', 'russ.abbott@gmail.com');
INSERT INTO "Users" ("Username", "FirstName", "LastName", "Email") VALUES ('zye5', 'Zilong', 'Ye', 'zye5@calstatela.edu');

INSERT INTO "Departments" ("Name", "ChairId") VALUES ('Computer Science', 10001);