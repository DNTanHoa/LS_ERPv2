UPDATE [PROBOM]
	SET ProductionQuantity = [JOB].[ProductionQuantity]
FROM [ProductionBOM] [PROBOM]
	 JOIN [JobHead] [JOB] ON [PROBOM].[JobHeadNumber] = [JOB].[Number]