create table address
(
	id serial not null
		constraint address_pk
			primary key,
	streetnumber text,
	postalcode text not null,
	city text,
	country text,
	hascoordinates boolean default false not null,
	latitude numeric,
	longitude numeric,
	street text
);

alter table address owner to postgres;

create unique index address_id_uindex
	on address (id);


const offerTable = create table offer
(
	id serial not null
		constraint offer_pkey
			primary key,
	name text not null,
	mail text not null,
	phone text,
	organisation text not null,
	ispublic boolean default false not null,
	address_id integer
		constraint provider_address_id_fk
			references address
				on update cascade on delete cascade,
	consumable_ids integer[],
	device_ids integer[],
	personal_ids integer[],
	token text not null,
	timestamp timestamp not null
);

alter table offer owner to postgres;


create table consumable
(
	id serial not null
		constraint consumables_pk
			primary key,
	category text not null,
	name text not null,
	manufacturer text,
	ordernumber text,
	amount integer default 0 not null,
	offer_id integer not null
		constraint consumable_offer_id_fk
			references offer,
	address_id integer not null
		constraint consumable_address_id_fk
			references address
				on update cascade on delete cascade,
	unit text,
	annotation text
);

alter table consumable owner to postgres;

create unique index consumables_id_uindex
    on consumable (id);


create table device
(
	category text not null,
	name text not null,
	id serial not null
		constraint device_pk
			primary key,
	manufacturer text not null,
	ordernumber text not null,
	amount integer default 0 not null,
	offer_id integer not null
		constraint device_offer_id_fk
			references offer,
	address_id integer not null
		constraint device_address_id_fk
			references address
				on update cascade on delete cascade,
	annotation text
);

alter table device owner to postgres;

create unique index device_id_uindex
	on device (id);


create table personal
(
	id serial not null
		constraint manpower_pk
			primary key,
	qualification text not null,
	institution text not null,
	researchgroup text,
	area text not null,
	experience_rt_pcr boolean default false not null,
	annotation text,
	offer_id integer
		constraint personal_offer_id_fk
			references offer,
	address_id integer
		constraint personal_address_id_fk
			references address
				on update cascade on delete cascade
);

alter table personal owner to postgres;

create unique index manpower_id_uindex
	on personal (id);


INSERT INTO address 
(id, streetnumber, postalcode, city, country, hascoordinates, latitude, longitude, street) 
VALUES (1, '77', '27498', 'Helgoland', 'Deutschland', true, 54.1830721, 7.8863887, 'Hauptstraße');


INSERT INTO offer 
(id, name, mail, phone, organisation, ispublic, address_id, consumable_ids, device_ids, personal_ids, token, timestamp) 
VALUES (1, 'Störtebeker', 'pirat.hilfsmittel@gmail.com', '987654', 'Instiut für Piraterie', true, 1, '{1}', '{1}', '{1}', 'FAUWc7MO4MM5M1upNhxUkZ9aArdlVo', 
'2020-03-29 17:58:12.041110');


INSERT INTO address 
(id, streetnumber, postalcode, city, country, hascoordinates, latitude, longitude, street) 
VALUES (2, '77', '27498', 'Helgoland', 'Deutschland', true, 54.1830721, 7.8863887, 'Hauptstraße');


INSERT INTO consumable 
(id, category, name, manufacturer, ordernumber, amount, offer_id, address_id, unit, annotation) 
VALUES (1, 'Rum', 'Nordrum', 'Störtebeker & Co', '999', 100, 1, 2, 'Liter', 'Arrr');


INSERT INTO address 
(id, streetnumber, postalcode, city, country, hascoordinates, latitude, longitude, street) 
VALUES (3, '77', '27498', 'Helgoland', 'Deutschland', true, 54.1830721, 7.8863887, 'Hauptstraße');


INSERT INTO personal 
(id, qualification, institution, researchgroup, area, experience_rt_pcr, annotation, offer_id, address_id) 
VALUES (1, 'Kapitän', 'Institut für Piraterie', 'Piraten Ahoi', 'Schiffsfahrt, Piraterie', false, 'Ahoi!', 1, 3);


INSERT INTO address 
(id, streetnumber, postalcode, city, country, hascoordinates, latitude, longitude, street) 
VALUES (4, '77', '27498', 'Helgoland', 'Deutschland', true, 54.1830721, 7.8863887, 'Hauptstraße');


INSERT INTO device 
(category, name, id, manufacturer, ordernumber, amount, offer_id, address_id, annotation) 
VALUES ('Schiffsmaterial', 'Steuerrad', 1, 'Störtebeker & Co', '12345', 10, 1, 4, 'Volle Fahrt voraus!');
