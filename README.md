docker-compose up

docker-compose exec mysql bash -c "mysql -u root -p\$MYSQL_ROOT_PASSWORD"

create database cardb;

use cardb;

create table cars
(id int unsigned auto_increment primary key,
name varchar(50),
color varchar(15),
price int,
creation_time datetime default current_timestamp,
modification_time datetime on update current_timestamp);

insert into cars(name, color, price) values("Fusca", "Amarelo", 50.000);
update cars set price = 17 where name = "Fusca";
delete from cars where id = 1;

select * from cars;
+----+-------+---------+-------+---------------------+-------------------+
| id | name | color | price | creation_time | modification_time |
+----+-------+---------+-------+---------------------+-------------------+
| 1 | Fusca | Amarelo | 50 | 2020-04-10 16:20:53 | NULL |
+----+-------+---------+-------+---------------------+-------------------+
1 row in set (0.00 sec)

- Criando um CDM e configurando-o
- Vamos abrir um novo terminal e executar:

curl -L -X POST 'localhost:8083/connectors/' -H 'Content-Type: application/json' --data-raw '{
"name": "cardb-connector",
"config": {
"connector.class": "io.debezium.connector.mysql.MySqlConnector",
"tasks.max": "1",
"database.hostname": "mysql",
"database.port": "3306",
"database.user": "root",
"database.password": "root",
"database.server.id": "223345",
"database.server.name": "mysql",
"database.whitelist": "cardb",
"database.history.kafka.bootstrap.servers": "kafka:9092",
"database.history.kafka.topic": "dbhistory.cardb",
"transforms": "unwrap",
"transforms.unwrap.type": "io.debezium.transforms.UnwrapFromEnvelope",
"transforms.unwrap.drop.tombstones": "false",
"key.converter": "org.apache.kafka.connect.json.JsonConverter",
"key.converter.schemas.enable": "false",
"value.converter": "org.apache.kafka.connect.json.JsonConverter",
"value.converter.schemas.enable": "false",
"include.schema.changes": "false"
}
}'

-- Para verficar se as configurações foram criadas, executamos:
curl localhost:8083/connectors/mystore-connector/status

---------Image

-- Para que possamos ver as mensagens geradas ao realizarmos alguma operação na tabela cars do mysql, na raiz do projeto, executaremos:

docker-compose exec kafka bash
kafka-console-consumer --bootstrap-server kafka:9092 --from-beginning --topic mysql.cardb.cars --property print.key=true --property key.separator="-"
