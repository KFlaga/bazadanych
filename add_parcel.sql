select * from przesylki;

SELECT * FROM KLIENCI;

select * from rodzaje_przesylek;

select * from dostawcy;

insert into przesylki
( id, czy_zabezpieczona, czy_ubezpieczona, nadawca, odbiorca, status, rodzaj,
data_nadania, data_odbioru, magazyn_docelowy, adres_docelowy, aktualny_dostawca )
values
(2, 'NIE', 'TAK', 1, 7, 'Zlecona przez klienta', 1, '20150510', null, 1, 'Komuny Paryskiej 2', 1);

select * from dostawcy;

SELECT ID FROM SYS.DOSTAWCY D WHERE D.DANE_PRACOWNIKA IN (SELECT ID FROM SYS.DANE_PRACOWNIKOW DP WHERE DP.ID = 1);

SELECT * FROM SYS.PRZESYLKI P WHERE P.AKTUALNY_DOSTAWCA = 1;

select * from klienci;

alter table przesylki
rename column Nr_zlecenia to Id;