-- Fictional demonstration events, not official university announcements.
-- Only seed an empty catalog. Never modify existing event dates or registrations.
IF NOT EXISTS (SELECT 1 FROM dbo.Events)
BEGIN
    DECLARE @day DATETIMEOFFSET(0) = TODATETIMEOFFSET(
        CAST(CAST(SWITCHOFFSET(SYSDATETIMEOFFSET(), '+08:00') AS DATE) AS DATETIME2), '+08:00');
    INSERT INTO dbo.Events (Title, Description, Location, StartsAt, Capacity) VALUES
    (N'Build Something Good', N'A hands-on campus hackathon. Bring an idea, meet a team, and build a small solution that makes student life better.', N'Innovation Lab', DATEADD(HOUR, 9, DATEADD(DAY, 3, @day)), 40),
    (N'Beyond the Classroom', N'An open conversation with alumni about first jobs, unexpected turns, and finding your own path in technology.', N'University Auditorium', DATEADD(HOUR, 14, DATEADD(DAY, 5, @day)), 120),
    (N'A Greener Tomorrow', N'Spend a morning growing something together. Join fellow students for a campus planting and sustainability workshop.', N'Campus Gardens', DATEADD(HOUR, 8, DATEADD(DAY, 7, @day)), 30),
    (N'Design for Everyone', N'Explore how thoughtful interfaces make everyday experiences more accessible. A beginner-friendly design workshop.', N'Computer Laboratory 2', DATEADD(HOUR, 13, DATEADD(DAY, 9, @day)), 25),
    (N'Campus in Color', N'A student-led showcase of illustration, photography, and creative work. Come for the art, stay for the conversations.', N'Student Center', DATEADD(HOUR, 10, DATEADD(DAY, 12, @day)), 80),
    (N'Find Your People', N'Meet student organizations and discover a new interest at this relaxed afternoon community gathering.', N'University Courtyard', DATEADD(HOUR, 15, DATEADD(DAY, 14, @day)), 100);
END;
