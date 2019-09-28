ALTER SEQUENCE "Users_Id_seq" RESTART WITH 10000;
ALTER SEQUENCE "Departments_Id_seq" RESTART WITH 1000;
ALTER SEQUENCE "Searches_Id_seq" RESTART WITH 10000;
ALTER SEQUENCE "Files_Id_seq" RESTART WITH 1000000;
ALTER SEQUENCE "Revisions_Id_seq" RESTART WITH 1000000;
ALTER SEQUENCE "Documents_Id_seq" RESTART WITH 1000000;
ALTER SEQUENCE "ApplicationTemplates_Id_seq" RESTART WITH 10000;

INSERT INTO "Users" ("Username", "FirstName", "LastName", "Email", "IsAdmin", "IsSysAdmin") VALUES ('csun', 'Chengyu', 'Sun', 'csun@calstatela.edu', 't', 't');
INSERT INTO "Users" ("Username", "FirstName", "LastName", "Email") VALUES ('eykang', 'Elaine', 'Kang', 'eykang@calstatela.edu');
INSERT INTO "Users" ("Username", "FirstName", "LastName", "Email") VALUES ('rpamula', 'Raj', 'Pamula', 'rpamula@calstatela.edu');
INSERT INTO "Users" ("Username", "FirstName", "LastName", "Email") VALUES ('rabbott', 'Russ', 'Abbott', 'russ.abbott@gmail.com');
INSERT INTO "Users" ("Username", "FirstName", "LastName", "Email") VALUES ('zye5', 'Zilong', 'Ye', 'zye5@calstatela.edu');

INSERT INTO "Departments" ("Name", "ChairId") VALUES ('Computer Science', 10001);

INSERT INTO "ApplicationTemplates" ("Name", "NumberOfReferences") VALUES ('Assistant Professor (2019-2020)', 3);
INSERT INTO "ApplicationTemplateDegrees" ("ApplicationTemplateId", "Index", "Name") VALUES (10000, 0, 'Ph.D.');
INSERT INTO "ApplicationTemplateDocuments" ("ApplicationTemplateId", "Index", "Name") VALUES (10000, 0, 'Application Form');
INSERT INTO "ApplicationTemplateDocuments" ("ApplicationTemplateId", "Index", "Name", "Description") VALUES (10000, 1, 'Cover Letter',
    'Cover letter should address minimum and preferred qualifications');
INSERT INTO "ApplicationTemplateDocuments" ("ApplicationTemplateId", "Index", "Name") VALUES (10000, 2, 'Curriculum Vitae');
INSERT INTO "ApplicationTemplateDocuments" ("ApplicationTemplateId", "Index", "Name", "Description") VALUES (10000, 3, 'Narrative Statement',
    'Narrative statement should describe your commitment to working effectively with faculty, '
    'staff, and students in a multicultural/multiethnic urban campus environment with a substantial population of first-generation students.');
