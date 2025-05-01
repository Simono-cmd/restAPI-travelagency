-- Create tables
CREATE TABLE Client (
    IdClient INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(120) NOT NULL,
    LastName NVARCHAR(120) NOT NULL,
    Email NVARCHAR(120) NOT NULL,
    Telephone NVARCHAR(120) NULL,
    Pesel NVARCHAR(120) NULL
);

CREATE TABLE Trip (
    IdTrip INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(120) NOT NULL,
    Description NVARCHAR(220) NULL,
    DateFrom DATETIME NOT NULL,
    DateTo DATETIME NOT NULL,
    MaxPeople INT NOT NULL
);

CREATE TABLE Country (
    IdCountry INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(120) NOT NULL
);

CREATE TABLE Country_Trip (
    IdCountry INT NOT NULL,
    IdTrip INT NOT NULL,
    PRIMARY KEY (IdCountry, IdTrip),
    FOREIGN KEY (IdCountry) REFERENCES Country(IdCountry),
    FOREIGN KEY (IdTrip) REFERENCES Trip(IdTrip)
);

CREATE TABLE Client_Trip (
    IdClient INT NOT NULL,
    IdTrip INT NOT NULL,
    RegisteredAt INT NOT NULL,
    PaymentDate INT NULL,
    PRIMARY KEY (IdClient, IdTrip),
    FOREIGN KEY (IdClient) REFERENCES Client(IdClient),
    FOREIGN KEY (IdTrip) REFERENCES Trip(IdTrip)
);

-- Insert sample data
-- Countries
INSERT INTO Country (Name) VALUES 
('Spain'),
('Italy'),
('France'),
('Greece'),
('Portugal'),
('Germany'),
('Croatia'),
('Turkey'),
('Egypt'),
('Thailand');

-- Trips
INSERT INTO Trip (Name, Description, DateFrom, DateTo, MaxPeople) VALUES
('Mediterranean Cruise', 'Explore the beautiful Mediterranean coast', '2025-06-15', '2025-06-25', 100),
('Paris Weekend', 'Romantic weekend in the city of love', '2025-05-10', '2025-05-12', 20),
('Greek Island Hopping', 'Visit multiple Greek islands in one trip', '2025-07-01', '2025-07-12', 30),
('Tuscany Wine Tour', 'Taste the finest wines of Tuscany', '2025-09-05', '2025-09-12', 15),
('Egyptian Pyramids', 'Explore ancient Egyptian history', '2025-10-10', '2025-10-20', 25),
('Thai Beach Retreat', 'Relax on pristine Thai beaches', '2025-11-15', '2025-11-29', 40),
('Alpine Skiing', 'Ski trip to the Alps', '2026-01-10', '2026-01-17', 30),
('Spanish Fiesta', 'Experience Spanish culture and nightlife', '2025-08-01', '2025-08-10', 25),
('Adriatic Sailing', 'Sail along the Adriatic coastline', '2025-06-20', '2025-06-30', 12),
('Turkish Delight', 'Cultural tour of Turkey', '2025-05-20', '2025-05-30', 22);

-- Country_Trip connections
INSERT INTO Country_Trip (IdCountry, IdTrip) VALUES
-- Mediterranean Cruise (Spain, Italy, France)
(1, 1), (2, 1), (3, 1),
-- Paris Weekend (France)
(3, 2),
-- Greek Island Hopping (Greece)
(4, 3),
-- Tuscany Wine Tour (Italy)
(2, 4),
-- Egyptian Pyramids (Egypt)
(9, 5),
-- Thai Beach Retreat (Thailand)
(10, 6),
-- Alpine Skiing (Germany, France)
(6, 7), (3, 7),
-- Spanish Fiesta (Spain)
(1, 8),
-- Adriatic Sailing (Croatia)
(7, 9),
-- Turkish Delight (Turkey)
(8, 10);

-- Clients
INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) VALUES
('Jan', 'Kowalski', 'jan.kowalski@example.com', '+48123456789', '90010112345'),
('Anna', 'Nowak', 'anna.nowak@example.com', '+48234567890', '92020223456'),
('Piotr', 'Wiśniewski', 'piotr.wisniewski@example.com', '+48345678901', '85030334567'),
('Katarzyna', 'Dąbrowska', 'katarzyna.dabrowska@example.com', '+48456789012', '88040445678'),
('Michał', 'Lewandowski', 'michal.lewandowski@example.com', '+48567890123', '87050556789'),
('Magdalena', 'Wójcik', 'magdalena.wojcik@example.com', '+48678901234', '91060667890'),
('Tomasz', 'Kamiński', 'tomasz.kaminski@example.com', '+48789012345', '86070778901'),
('Aleksandra', 'Kowalczyk', 'aleksandra.kowalczyk@example.com', '+48890123456', '94080889012'),
('Marcin', 'Zieliński', 'marcin.zielinski@example.com', '+48901234567', '89090990123'),
('Monika', 'Szymańska', 'monika.szymanska@example.com', '+48012345678', '93101001234');

-- Client_Trip registrations 
-- Note: RegisteredAt and PaymentDate should probably be datetime fields,
-- but I'm following the schema provided which shows them as INT
INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate) VALUES
(1, 1, 20250501, 20250505),   -- Jan on Mediterranean Cruise
(1, 3, 20250510, 20250515),   -- Jan on Greek Island Hopping
(2, 2, 20250415, 20250417),   -- Anna on Paris Weekend
(2, 8, 20250620, NULL),       -- Anna on Spanish Fiesta (not paid yet)
(3, 5, 20250601, 20250610),   -- Piotr on Egyptian Pyramids
(4, 6, 20250715, 20250720),   -- Katarzyna on Thai Beach Retreat
(5, 4, 20250801, 20250805),   -- Michał on Tuscany Wine Tour
(6, 7, 20251101, 20251110),   -- Magdalena on Alpine Skiing
(7, 9, 20250501, 20250510),   -- Tomasz on Adriatic Sailing
(8, 10, 20250301, 20250310),  -- Aleksandra on Turkish Delight
(9, 1, 20250502, 20250512),   -- Marcin on Mediterranean Cruise
(10, 3, 20250601, 20250615); -- Monika on Greek Island Hopping