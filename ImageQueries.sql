select
	i.ContentId,
	t.Tag,
	t.Confidence
from [Image] i
	cross apply openjson (i.ImageAnalysis, N'$.tags')
		with (
			Tag nvarchar(100) N'$.name', 
			Confidence decimal(5,4)  N'$.confidence'
		) as t	
where ISJSON(ImageAnalysis) > 0
	and t.Confidence > 0.85

select
	i.ContentId,
	i.Localization,
	i.[Description],
	i.Width,
	i.Height,
	i.[Format],
	(
		select 
			t.Tag,
			t.Confidence
		from openjson (i.ImageAnalysis, N'$.tags')
		with (
			Tag nvarchar(100) N'$.name', 
			Confidence decimal(5,4)  N'$.confidence'
		) as t	
		where ISJSON(i.ImageAnalysis) > 0
			and t.Confidence > 0.89
		for json path
	) as [Tags]
from [Image] i
for json path
