--delete from BookingRequest;
--delete from Kilos;
--delete from MarketPlace;
--delete from [Message];
--delete from Notif
--delete from Payment
--delete from Post
--delete from PostComment
--delete from PostPhoto
--delete from Room
--delete from RoomMember
--delete from Schedules
--delete from [Transaction]
--delete from UserLikedPost

--use Wiggly

--select * from MarketPlace

--select * from BookingRequest

--select * from [Transaction]

--select * from LivestockType

--select * from MarketplaceItemLivestock

select * from BookingRequest
select * from BookingRequestSubItem

select * from [Transaction]
select * from TransactionSubItem

select * from notif


-----delete boooking notif and transactions
delete from BookingRequest
delete from BookingRequestSubItem
delete from Notif
delete from [Transaction]
delete from TransactionSubItem


select * from MarketplaceItemLivestock where MarketplaceItem like  '344a12ef-5c18-4d53-932e-fa372b641a6b'
