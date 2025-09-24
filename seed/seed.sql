-- -- Create temporary table matching ALL CSV columns
-- CREATE TABLE temp_noise_data (
--     Time TIMESTAMP,
--     LCPK DECIMAL,
--     LAEQ DECIMAL,
--     LZPK DECIMAL,
--     "LAVG (Q5)" DECIMAL,
--     LCEQ DECIMAL,
--     LAFmax DECIMAL,
--     LASmax DECIMAL,
--     "LAVG (Q3)" DECIMAL,
--     Motion INTEGER,
--     MotionSeries TEXT,
--     Paused INTEGER,
--     PausedSeries TEXT
-- );

-- -- Import to temp table
-- COPY temp_noise_data FROM '/seed/NoiseData.csv' DELIMITER ',' CSV HEADER;

-- Insert into final table with correct column name (LavgQ3, not lavgq3)
INSERT INTO NoiseData (id, Time, LavgQ3)
SELECT gen_random_uuid(), Time, "LAVG (Q3)"
FROM temp_noise_data;

-- Drop temporary table
