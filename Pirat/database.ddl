--
-- PostgreSQL database dump
--

-- Dumped from database version 10.12 (Ubuntu 10.12-0ubuntu0.18.04.1)
-- Dumped by pg_dump version 10.12 (Ubuntu 10.12-0ubuntu0.18.04.1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET default_with_oids = false;

--
-- Name: address; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.address (
    id integer NOT NULL,
    streetnumber text,
    postalcode text NOT NULL,
    city text,
    country text,
    hascoordinates boolean DEFAULT false NOT NULL,
    latitude numeric,
    longitude numeric,
    street text
);


--
-- Name: address_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.address_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: address_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.address_id_seq OWNED BY public.address.id;


--
-- Name: consumable; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.consumable (
    id integer NOT NULL,
    category text NOT NULL,
    name text NOT NULL,
    manufacturer text,
    ordernumber text,
    amount integer DEFAULT 0 NOT NULL,
    offer_id integer NOT NULL,
    address_id integer NOT NULL,
    unit text,
    annotation text
);


--
-- Name: consumable_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.consumable_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: consumable_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.consumable_id_seq OWNED BY public.consumable.id;


--
-- Name: device; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.device (
    category text NOT NULL,
    name text NOT NULL,
    id integer NOT NULL,
    manufacturer text NOT NULL,
    ordernumber text NOT NULL,
    amount integer DEFAULT 0 NOT NULL,
    offer_id integer NOT NULL,
    address_id integer NOT NULL,
    annotation text
);


--
-- Name: device_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.device_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: device_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.device_id_seq OWNED BY public.device.id;


--
-- Name: personal; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.personal (
    id integer NOT NULL,
    qualification text NOT NULL,
    institution text NOT NULL,
    researchgroup text,
    area text NOT NULL,
    experience_rt_pcr boolean DEFAULT false NOT NULL,
    annotation text,
    offer_id integer,
    address_id integer
);


--
-- Name: manpower_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.manpower_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: manpower_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.manpower_id_seq OWNED BY public.personal.id;


--
-- Name: offer; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.offer (
    id integer NOT NULL,
    name text NOT NULL,
    mail text NOT NULL,
    phone text NOT NULL,
    organisation text NOT NULL,
    ispublic boolean DEFAULT false NOT NULL,
    address_id integer,
    consumable_ids integer[],
    device_ids integer[],
    personal_ids integer[],
    token text NOT NULL,
    "timestamp" timestamp without time zone
);


--
-- Name: offer_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.offer_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: offer_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.offer_id_seq OWNED BY public.offer.id;


--
-- Name: address id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.address ALTER COLUMN id SET DEFAULT nextval('public.address_id_seq'::regclass);


--
-- Name: consumable id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.consumable ALTER COLUMN id SET DEFAULT nextval('public.consumable_id_seq'::regclass);


--
-- Name: device id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.device ALTER COLUMN id SET DEFAULT nextval('public.device_id_seq'::regclass);


--
-- Name: offer id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.offer ALTER COLUMN id SET DEFAULT nextval('public.offer_id_seq'::regclass);


--
-- Name: personal id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.personal ALTER COLUMN id SET DEFAULT nextval('public.manpower_id_seq'::regclass);


--
-- Name: address address_pk; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.address
    ADD CONSTRAINT address_pk PRIMARY KEY (id);


--
-- Name: consumable consumables_pk; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.consumable
    ADD CONSTRAINT consumables_pk PRIMARY KEY (id);


--
-- Name: device device_pk; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.device
    ADD CONSTRAINT device_pk PRIMARY KEY (id);


--
-- Name: personal manpower_pk; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.personal
    ADD CONSTRAINT manpower_pk PRIMARY KEY (id);


--
-- Name: offer offer_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.offer
    ADD CONSTRAINT offer_pkey PRIMARY KEY (id);


--
-- Name: address_id_uindex; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX address_id_uindex ON public.address USING btree (id);


--
-- Name: consumables_id_uindex; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX consumables_id_uindex ON public.consumable USING btree (id);


--
-- Name: device_id_uindex; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX device_id_uindex ON public.device USING btree (id);


--
-- Name: manpower_id_uindex; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX manpower_id_uindex ON public.personal USING btree (id);


--
-- Name: consumable consumable_address_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.consumable
    ADD CONSTRAINT consumable_address_id_fk FOREIGN KEY (address_id) REFERENCES public.address(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: consumable consumable_offer_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.consumable
    ADD CONSTRAINT consumable_offer_id_fk FOREIGN KEY (offer_id) REFERENCES public.offer(id);


--
-- Name: device device_address_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.device
    ADD CONSTRAINT device_address_id_fk FOREIGN KEY (address_id) REFERENCES public.address(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: device device_offer_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.device
    ADD CONSTRAINT device_offer_id_fk FOREIGN KEY (offer_id) REFERENCES public.offer(id);


--
-- Name: personal personal_address_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.personal
    ADD CONSTRAINT personal_address_id_fk FOREIGN KEY (address_id) REFERENCES public.address(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: personal personal_offer_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.personal
    ADD CONSTRAINT personal_offer_id_fk FOREIGN KEY (offer_id) REFERENCES public.offer(id);


--
-- Name: offer provider_address_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.offer
    ADD CONSTRAINT provider_address_id_fk FOREIGN KEY (address_id) REFERENCES public.address(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--


create table demand
(
	id serial
		constraint demand_pk
			primary key,
	institution text,
	name text,
	mail text not null,
	phone text,
	address_id int
		constraint demand_address_id_fk
			references address,
	comment text,
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
	comment text,
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
	comment text,
	created_at_timestamp timestamp not null,
	is_deleted boolean not null
);
