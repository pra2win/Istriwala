--FOREIGN KEYs
ALTER TABLE [dbo].[OrderPickupDtls]  WITH CHECK ADD  CONSTRAINT [FK_OrderPickupDtls_Users] 
FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])


ALTER TABLE [dbo].[OrderPickupDtls]  WITH CHECK ADD  CONSTRAINT [FK_OrderPickupDtls_OrderHeader] 
FOREIGN KEY([OrderId])
REFERENCES [dbo].[OrderHeader] ([OrderId])


ALTER TABLE [dbo].[OrderDelivery]  WITH CHECK ADD  CONSTRAINT [FK_OrderDelivery_OrderHeader] 
FOREIGN KEY([OrderId])
REFERENCES [dbo].[OrderHeader] ([OrderId])


ALTER TABLE [dbo].[OrderDelivery]  WITH CHECK ADD  CONSTRAINT [FK_OrderDelivery_Users] 
FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])


