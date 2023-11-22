USE esp32db;

-- Create a table to store temperature and humidity data
CREATE TABLE IF NOT EXISTS SensorData (
    id INT AUTO_INCREMENT PRIMARY KEY,
    espId INT NOT NULL,
    userId INT NOT NULL,
    temperature DECIMAL(5,2) NOT NULL,
    humidity DECIMAL(5,2) NOT NULL,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
