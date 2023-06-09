  DECLARE @startDate DATETIME = GETDATE();

INSERT INTO [TrainReservationSystem_DB].[dbo].[TrainDetails] (TrainName, TrainId, Origin, Destination, Departure, Arrival, SeatCapacity, SeatRate)
VALUES 
('Shatabdi Express', 12001, 'New Delhi', 'Lucknow', DATEADD(HOUR, 4, @startDate), DATEADD(HOUR, 8, @startDate), 50, 500),
('Rajdhani Express', 12424, 'Mumbai', 'New Delhi', DATEADD(HOUR, 8, @startDate), DATEADD(HOUR, 16, @startDate), 60, 600),
('Garib Rath Express', 12215, 'Kolkata', 'New Delhi', DATEADD(HOUR, 10, @startDate), DATEADD(HOUR, 20, @startDate), 70, 700),
('Duronto Express', 12269, 'Chennai', 'Delhi', DATEADD(HOUR, 12, @startDate), DATEADD(HOUR, 24, @startDate), 80, 800),
('Jan Shatabdi Express', 12057, 'Jaipur', 'New Delhi', DATEADD(HOUR, 14, @startDate), DATEADD(HOUR, 18, @startDate), 90, 900),
('Swarna Shatabdi Express', 12029, 'Chandigarh', 'New Delhi', DATEADD(HOUR, 16, @startDate), DATEADD(HOUR, 20, @startDate), 50, 500),
('Humsafar Express', 22920, 'Ahmedabad', 'Chennai', DATEADD(HOUR, 18, @startDate), DATEADD(HOUR, 38, @startDate), 60, 600),
('Sampark Kranti Express', 12217, 'Kochi', 'Delhi', DATEADD(HOUR, 20, @startDate), DATEADD(HOUR, 50, @startDate), 70, 700),
('Gatimaan Express', 12049, 'Agra', 'New Delhi', DATEADD(HOUR, 22, @startDate), DATEADD(HOUR, 24, @startDate), 80, 800),
('Tejas Express', 22120, 'Mumbai', 'Goa', DATEADD(HOUR, 24, @startDate), DATEADD(HOUR, 30, @startDate), 90, 900),
('Rajya Rani Express', 16561, 'Bangalore', 'Mysore', DATEADD(HOUR, 26, @startDate), DATEADD(HOUR, 28, @startDate), 50, 500),
('Punjab Mail Express', 12137, 'Mumbai', 'Firozpur', DATEADD(HOUR, 28, @startDate), DATEADD(HOUR, 60, @startDate), 60, 600),
('Sarnath Express', 15159, 'Chhapra', 'Durg', DATEADD(HOUR, 30, @startDate), DATEADD(HOUR, 60, @startDate), 70, 700),
('Gorakhpur Express', 15047, 'Mumbai', 'Gorakhpur', DATEADD(HOUR, 32, @startDate), DATEADD(HOUR, 70, @startDate), 80, 800),
('Rapti Sagar Express', 12511, 'Gorakhpur', 'Trivandrum', DATEADD(HOUR, 34, @startDate), DATEADD(HOUR, 90, @startDate), 90, 1200),
('Kaveri Express', 16021, 'Mysore', 'Chennai', DATEADD(HOUR, 36, @startDate), DATEADD(HOUR, 60, @startDate), 70, 700),
('Jammu Tawi Express', 13151, 'Mumbai', 'Jammu', DATEADD(HOUR, 38, @startDate), DATEADD(HOUR, 90, @startDate), 80, 800),
('Himalayan Queen Express', 14095, 'Kalka', 'Shimla', DATEADD(HOUR, 40, @startDate), DATEADD(HOUR, 46, @startDate), 50, 500),
('Kalinga Utkal Express', 18477, 'Puri', 'Haridwar', DATEADD(HOUR, 42, @startDate), DATEADD(HOUR, 96, @startDate), 60, 600),
('Chennai Express', 12680, 'Bangalore', 'Chennai', DATEADD(HOUR, 44, @startDate), DATEADD(HOUR, 60, @startDate), 70, 700),
('Mangalore Express', 16346, 'Trivandrum', 'Mangalore', DATEADD(HOUR, 46, @startDate), DATEADD(HOUR, 72, @startDate), 80, 800),
('Godavari Express', 12727, 'Mumbai', 'Visakhapatnam', DATEADD(HOUR, 48, @startDate), DATEADD(HOUR, 84, @startDate), 90, 900),
('Karnataka Express', 12627, 'New Delhi', 'Bangalore', DATEADD(HOUR, 50, @startDate), DATEADD(HOUR, 78, @startDate), 50, 500),
('Karnavati Express', 12933, 'Mumbai', 'Ahmedabad', DATEADD(HOUR, 52, @startDate), DATEADD(HOUR, 62, @startDate), 60, 600),
('Kamayani Express', 11071, 'Mumbai', 'Varanasi', DATEADD(HOUR, 54, @startDate), DATEADD(HOUR, 96, @startDate), 70, 700),
('Visakhapatnam - Secunderabad Vande Bharat Express', 20833, 'Visakhapatnam', 'Secunderabad', DATEADD(HOUR, 54, @startDate), DATEADD(HOUR, 96, @startDate), 100, 700),
('Secunderabad - Visakhapatnam Vande Bharat Express', 20834, 'Secunderabad', 'Visakhapatnam', DATEADD(HOUR, 36, @startDate), DATEADD(HOUR, 72, @startDate), 100, 700),
('Varanasi - New Delhi Vande Bharat Express', 22435, 'Varanasi', 'New Delhi', DATEADD(HOUR, 72, @startDate), DATEADD(HOUR, 108, @startDate), 500, 1200),
('New Delhi - Varanasi Vande Bharat Express', 22436, 'New Delhi', 'Varanasi', DATEADD(HOUR, 48, @startDate), DATEADD(HOUR, 84, @startDate), 500, 1200),
('New Delhi - SMVD Katra Vande Bharat Express', 22439, 'New Delhi', 'SMVD Katra', DATEADD(HOUR, 72, @startDate), DATEADD(HOUR, 120, @startDate), 600, 1300),
('SMVD Katra - New Delhi Vande Bharat Express', 22440, 'SMVD Katra', 'New Delhi', DATEADD(HOUR, 72, @startDate), DATEADD(HOUR, 120, @startDate), 600, 1300),
('Mumbai Central - Gandhinagar CAP Vande Bharat Express', 20901, 'Mumbai Central', 'Gandhinagar', DATEADD(HOUR, 60, @startDate), DATEADD(HOUR, 120, @startDate), 550, 1300),
('Gandhinagar CAP - Mumbai Central Vande Bharat Express', 20902, 'Gandhinagar', 'Mumbai Central', DATEADD(HOUR, 54, @startDate), DATEADD(HOUR, 114, @startDate), 550, 1300),
('New Delhi - Amb Andaura Vande Bharat Express', 22447, 'New Delhi', 'Amb Andaura', DATEADD(HOUR, 60, @startDate), DATEADD(HOUR, 108, @startDate), 450, 1200),
('Amb Andaura - New Delhi Vande Bharat Express', 22448, 'Amb Andaura', 'New Delhi', DATEADD(HOUR, 60, @startDate), DATEADD(HOUR, 108, @startDate), 450, 1200),
('Chennai - Mysuru Vande Bharat Express', 20607, 'Chennai', 'Mysuru', DATEADD(HOUR, 66, @startDate), DATEADD(HOUR, 96, @startDate), 450, 1100),
('Mysuru - Chennai Vande Bharat Express', 20608, 'Mysuru', 'Chennai', DATEADD(HOUR, 72, @startDate), DATEADD(HOUR, 102, @startDate), 450, 1100),
('Bilaspur Junction - Nagpur Junction Vande Bharat Express', 20825, 'Bilaspur Junction', 'Nagpur Junction', DATEADD(HOUR, 54, @startDate), DATEADD(HOUR, 84, @startDate), 350, 900),
('Nagpur - Bilaspur Junction Vande Bharat Express', 20826, 'Nagpur Junction', 'Bilaspur Junction', DATEADD(HOUR, 60, @startDate), DATEADD(HOUR, 90, @startDate), 350, 900);
