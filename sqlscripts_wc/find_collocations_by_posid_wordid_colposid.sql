use WordCollocationTest
--1	3	3	2	noun	abandon	adjective	1
--2	3	4	5	verb	abandon	adverb	5
Select collocationid, colwordid,

(select entry from colwords where colwordid = collocations.colwordid) as ColWord

,CollocationPattern

from collocations
where posid=2 and wordId=3 and colposid=4


