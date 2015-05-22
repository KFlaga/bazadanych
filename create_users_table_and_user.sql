
create table UZYTKOWNICY (
ID number primary key,
LOGIN varchar(32) constraint userLOGINnotNull NOT NULL,
PASSWD varchar(32) constraint userPASSWDnotNull NOT NULL,
PERMISSION number not null
) tablespace COURIER;

insert into uzytkownicy
(id, login, passwd, permission)
VALUES (0, 'ardr', 'db123', 0);

