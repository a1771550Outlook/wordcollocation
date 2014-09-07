select Entry,

(select Entry from poss where (select posId from collocations where wcexamples.CollocationId = collocations.CollocationId)=poss.posId) as [Pos],
(select EntryChi from poss where (select posId from collocations where wcexamples.CollocationId = collocations.CollocationId)=poss.posId) as [PosChi],
(select EntryJap from poss where (select posId from collocations where wcexamples.CollocationId = collocations.CollocationId)=poss.posId) as [PosJap],
(select Entry from colposs where (select colposId from collocations where wcexamples.CollocationId = collocations.CollocationId)=colposs.colposId) as [ColPos],
(select EntryChi from colposs where (select colposId from collocations where wcexamples.CollocationId = collocations.CollocationId)=colposs.colposId) as [ColPosChi],
(select EntryJap from colposs where (select colposId from collocations where wcexamples.CollocationId = collocations.CollocationId)=colposs.colposId) as [ColPosJap],
(select Entry from words where (select wordId from collocations where wcexamples.CollocationId = collocations.CollocationId)=words.wordId) as [Word],
(select EntryChi from words where (select wordId from collocations where wcexamples.CollocationId = collocations.CollocationId)=words.wordId) as [WordChi],
(select EntryJap from words where (select wordId from collocations where wcexamples.CollocationId = collocations.CollocationId)=words.wordId) as [WordJap],

(select Entry from colwords where (select colwordId from collocations where wcexamples.CollocationId = collocations.CollocationId)=colwords.colwordId) as [ColWord],
(select EntryChi from colwords where (select colwordId from collocations where wcexamples.CollocationId = collocations.CollocationId)=colwords.colwordId) as [ColWordChi],
(select EntryJap from colwords where (select colwordId from collocations where wcexamples.CollocationId = collocations.CollocationId)=colwords.colwordId) as [ColWordJap],

(select CollocationPattern from collocations where wcexamples.CollocationId = collocations.CollocationId) as pattern


 from WcExamples;