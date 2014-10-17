CREATE PROCEDURE spGetChangedItemsAndSubscribers
	@dateFrom DATETIME = NULL
AS 
SELECT	i.Id AS itemId,
		ISNULL(nsub.Email, ISNULL(u.Email, p.Email)) AS Email,
		ich.LanguageId,
		ich.FirstChange
FROM	( SELECT	ich.ItemId,
					MAX(ich.DateOfEntry) AS DateOfEntry,
					MAX(ich.LanguageId) AS LanguageId,
					MIN(ich.DateOfEntry) AS FirstChange
		  FROM		dbo.ItemChangeHistory ich
		  WHERE		@dateFrom IS NULL
					OR ich.DateOfEntry >= @dateFrom
		  GROUP BY	ich.ItemId
		) ich
INNER JOIN dbo.Item i ON ich.ItemId = i.Id
CROSS APPLY ( SELECT	ns.NewsletterSubscriberId
			  FROM		dbo.NewsletterSubscription ns
			  INNER JOIN dbo.Item i2 ON ns.ItemId = i2.Id
			  WHERE		i.Node.IsDescendantOf(i2.Node) = 1
						AND ns.DateOfEntry <= ich.DateOfEntry
			) ns
INNER JOIN dbo.NewsletterSubscriber nsub ON nsub.Id = ns.NewsletterSubscriberId
LEFT JOIN [user] u ON nsub.userId = u.Id
LEFT JOIN [Partner] p ON nsub.partnerId = p.Id