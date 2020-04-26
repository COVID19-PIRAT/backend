create table address
(
	id serial not null
		constraint address_pk
			primary key,
	hascoordinates boolean default false not null,
	latitude numeric,
	longitude numeric,
	is_deleted boolean default false not null,
	street_line_1 text,
	street_line_2 text,
	street_line_3 text,
	street_line_4 text,
	county text,
	city text,
	state text,
	postal_code text,
	country text not null
);

create unique index address_id_uindex
	on address (id);

create table offer
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
	token text not null,
	timestamp timestamp not null,
	region text not null
);

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
			references offer
				on update cascade on delete cascade,
	unit text,
	annotation text,
	is_deleted boolean default false not null
);

create unique index consumables_id_uindex
	on consumable (id);

create table device
(
	category text not null,
	name text not null,
	id serial not null
		constraint device_pk
			primary key,
	manufacturer text,
	ordernumber text,
	amount integer default 0 not null,
	offer_id integer not null
		constraint device_offer_id_fk
			references offer
				on update cascade on delete cascade,
	annotation text,
	is_deleted boolean default false not null
);

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
			references offer
				on update cascade on delete cascade,
	is_deleted boolean default false not null
);

create unique index manpower_id_uindex
	on personal (id);

create table region_subscription
(
	id serial not null
		constraint region_subscription_pk
			primary key,
	email text not null,
	subscription_date timestamp default now() not null,
	active boolean default true not null,
	name text not null,
	institution text not null,
	postal_code text not null,
	latitude numeric not null,
	longitude numeric not null,
	region text not null
);

create table change
(
	id serial not null
		constraint change_pk
			primary key,
	element_type text not null,
	element_id integer not null,
	change_type text not null,
	timestamp timestamp not null,
	reason text,
	diff_amount integer default 0 not null,
	element_category text,
	element_name text,
	region text not null
);

create table demand
(
	id serial not null
		constraint demand_pk
			primary key,
	institution text,
	name text,
	mail text not null,
	phone text,
	address_id integer
		constraint demand_address_id_fk
			references address,
	annotation text,
	token text not null,
	created_at_timestamp timestamp not null
);

create index demand_token_index
	on demand (token);

create table demand_device
(
	id serial not null
		constraint demand_device_pk
			primary key,
	demand_id integer not null
		constraint demand_device_demand_id_fk
			references demand
				on update cascade on delete cascade,
	category text not null,
	name text,
	manufacturer text,
	amount integer not null,
	annotation text,
	created_at_timestamp timestamp not null,
	is_deleted boolean not null
);

create table demand_consumable
(
	id serial not null
		constraint demand_consumable_pk
			primary key,
	demand_id integer not null
		constraint demand_consumable_demand_id_fk
			references demand
				on update cascade on delete cascade,
	category text not null,
	name text,
	manufacturer text,
	amount integer not null,
	unit text not null,
	annotation text,
	created_at_timestamp timestamp not null,
	is_deleted boolean not null
);
