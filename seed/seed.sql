-- NOISE DATA

-- Drop temporary table if it exists
DELETE FROM "NoiseData";

-- Create temporary table matching ALL CSV columns
CREATE TABLE temp_noise_data (
    Time TIMESTAMP,
    LCPK DECIMAL,
    LAEQ DECIMAL,
    LZPK DECIMAL,
    "LAVG (Q5)" DECIMAL,
    LCEQ DECIMAL,
    LAFmax DECIMAL,
    LASmax DECIMAL,
    "LAVG (Q3)" DECIMAL,
    Motion INTEGER,
    MotionSeries TEXT,
    Paused INTEGER,
    PausedSeries TEXT
);

-- Import to temp table
COPY temp_noise_data FROM '/seed/NoiseData.csv' DELIMITER ',' CSV HEADER;

INSERT INTO "NoiseData" ("Id", "Time", "LavgQ3")
SELECT gen_random_uuid(), Time, "LAVG (Q3)"
FROM temp_noise_data;

DROP TABLE temp_noise_data;

-- VIBRATION DATA

-- Empty the table before seeding
DELETE FROM "VibrationData";

-- Create temporary table matching ALL CSV columns
CREATE TABLE temp_vibration_data (
    Date TEXT,
    ConnectedOn TEXT,
    DisconnectedOn TEXT,
    "Tag Vibration (m/s2)" DECIMAL,
    "Sensed Vibration (m/s2)" DECIMAL,
    TriggerTime TEXT,
    "TriggerTime (seconds)" INTEGER,
    "Tag Exposure Points" DECIMAL,
    "Sensed Exposure Points" DECIMAL,
    Overdose INTEGER,
    "EAV Level" INTEGER,
    "TEP EAV Reached" TEXT,
    "SEP EAV Reached" TEXT,
    "ELV Level" INTEGER,
    "TEP ELV Reached" TEXT,
    "SEP ELV Reached" TEXT,
    BaseStationID TEXT,
    Division TEXT,
    HAVUnitID TEXT,
    Manufacturer TEXT,
    Model TEXT,
    OperatorID TEXT,
    OperatorName TEXT,
    "Operator First Name" TEXT,
    "Operator Last Name" TEXT,
    Project TEXT,
    Region TEXT,
    TagID TEXT,
    ToolID TEXT,
    ToolName TEXT,
    "Removed From Rasor ID" TEXT,
    "Returned To Rasor ID" TEXT
);

-- Import to temp table
COPY temp_vibration_data FROM '/seed/VibrationData.csv' DELIMITER ',' CSV HEADER;

-- Insert data into VibrationData table with date format conversion
INSERT INTO "VibrationData" ("Id", "ConnectedOn", "Exposure", "DisconnectedOn")
SELECT 
    gen_random_uuid(),
    TO_TIMESTAMP(ConnectedOn, 'DD-MM-YYYY"T"HH24:MI:SS"Z"'),
    "Tag Exposure Points",
    TO_TIMESTAMP(DisconnectedOn, 'DD-MM-YYYY"T"HH24:MI:SS"Z"')
FROM temp_vibration_data
WHERE ConnectedOn IS NOT NULL AND DisconnectedOn IS NOT NULL;

DROP TABLE temp_vibration_data;

-- DUST DATA

-- Empty the table before seeding
DELETE FROM "DustData";

-- Create temporary table matching ALL CSV columns
CREATE TABLE temp_dust_data (
    Timestamp TEXT,
    "PM 1 Live" DECIMAL,
    "PM 1 STEL" DECIMAL,
    "PM 1 TWA" DECIMAL,
    "PM 2.5 Live" DECIMAL,
    "PM 2.5 STEL" DECIMAL,
    "PM 2.5 TWA" DECIMAL,
    "PM 4.25 Live" DECIMAL,
    "PM 4.25 STEL" DECIMAL,
    "PM 4.25 TWA" DECIMAL,
    "PM 10.0 Live" DECIMAL,
    "PM 10.0 STEL" DECIMAL,
    "PM 10.0 TWA" DECIMAL,
    "STEL Threshold" DECIMAL,
    "TWA Threshold" DECIMAL
);

-- Import to temp table
COPY temp_dust_data FROM '/seed/DustData.csv' DELIMITER ',' CSV HEADER;

INSERT INTO "DustData" ("Id", "Time", "PM1S", "PM25S", "PM4S", "PM10S", "PM1T", "PM25T", "PM4T", "PM10T")
SELECT 
    gen_random_uuid(),
    Timestamp::TIMESTAMP WITH TIME ZONE,
    "PM 1 STEL",
    "PM 2.5 STEL",
    "PM 4.25 STEL",
    "PM 10.0 STEL",
    "PM 1 TWA",
    "PM 2.5 TWA",
    "PM 4.25 TWA",
    "PM 10.0 TWA"
FROM temp_dust_data
WHERE Timestamp IS NOT NULL;

DROP TABLE temp_dust_data;