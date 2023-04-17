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
('Rapti Sagar Express', 12511, 'Gorakhpur', 'Trivandrum', DATEADD(HOUR, 34, @startDate), DATEADD(HOUR, 90, @startDate), 90, 1200);