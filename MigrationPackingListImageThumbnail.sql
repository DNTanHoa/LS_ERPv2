INSERT INTO [dbo].[PackingListImageThumbnail]
           ([PackingListID]
           ,[ImageUrl]
           ,[SortIndex]
           ,[CreatedBy]
           ,[CreatedAt])
     SELECT PackingList.ID
			,'https://dev-erp-api.datashareroom.com/logochung.png'
			,1
			,'admin'
			,GETDATE()
	 FROM PackingList
GO