# --------------------------- League
-- Create table 
CREATE TABLE leagues (
  id VARCHAR(5) UNIQUE PRIMARY
)

-- Modify columns
ALTER TABLE leagues
MODIFY  id VARCHAR(10) 


-- Add columns
ALTER TABLE leagues
ADD COLUMN name VARCHAR(100) NOT NULL;

ALTER TABLE leagues
ADD COLUMN logo_url VARCHAR(500);


# --------------------------- Teams

CREATE TABLE teams (
    id VARCHAR(10) NOT NULL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    logo VARCHAR(500),
    league_id VARCHAR(5),

    FOREIGN KEY (league_id) REFERENCES leagues(id)
        ON DELETE SET NULL
);
