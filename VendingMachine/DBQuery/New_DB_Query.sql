

create database Product_Store;



CREATE TABLE mst_product(
    product_id bigint NOT NULL AUTO_INCREMENT,
    product_name varchar(255),
    price decimal,
    img_path varchar(255),
    stock bigint,
    soldout bit,
    PRIMARY KEY (product_id)
);



insert into mst_product values(1,'Little Hearts',10,'',20,0);
insert into mst_product values(2,'Lays Green',10,'',20,0);
insert into mst_product values(3,'Bounce',10,'',20,0);
insert into mst_product values(4,'Dark Fantasy',10,'',20,0);
insert into mst_product values(5,'Lays Red',10,'',20,0);

CREATE TABLE paytm_upi (
  id int(11) NOT NULL AUTO_INCREMENT,
  order_date datetime DEFAULT NULL,
  order_id varchar(50) DEFAULT NULL,
  order_amount decimal(6,2) DEFAULT NULL,
  qr_status_code VARCHAR(10) DEFAULT NULL,
  qr_status_msg varchar(50) DEFAULT NULL,
  transaction_id varchar(50) DEFAULT NULL,
  transaction_date datetime DEFAULT NULL,
  transaction_code VARCHAR(10) DEFAULT NULL,
  transaction_msg varchar(50) DEFAULT NULL,
  sales_code varchar(20) DEFAULT NULL,
  is_refunded bit(1) NOT NULL DEFAULT b'0',
  refund_request_date datetime DEFAULT NULL,
  refund_request_id varchar(50) DEFAULT NULL,
  refund_request_amount decimal(6,2) DEFAULT NULL,
  refund_id varchar(50) DEFAULT NULL,
  refund_code VARCHAR(10) DEFAULT NULL,
  refund_msg varchar(50) DEFAULT NULL,
  PRIMARY KEY (id)
) 






















