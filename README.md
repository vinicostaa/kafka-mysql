docker-compose up

create table cars 
(id int unsigned auto_increment primary key, 
name varchar(50),
color varchar(15),
price int, 
creation_time datetime default current_timestamp, 
modification_time datetime on update current_timestamp);

insert into cars(name, color, price) values("Fusca", "Amarelo", 50.000);



- Criando um CDM e configurando-o

curl -L -X POST 'localhost:8083/connectors/' -H 'Content-Type: application/json' --data-raw '{
    "name": "mycars-connector",
    "config": {
        "connector.class": "io.debezium.connector.mysql.MySqlConnector",
        "tasks.max": "1",
        "database.hostname": "mysql",
        "database.port": "3306",
        "database.user": "root",
        "database.password": "root",
        "database.server.id": "223345",
        "database.server.name": "mysql",
        "database.whitelist": "mycars",
        "database.history.kafka.bootstrap.servers": "kafka:9092",
        "database.history.kafka.topic": "dbhistory.mycars",
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

curl -i -X POST -H "Accept:application/json" -H "Content-Type:application/json" localhost:8083/connectors/ -d '{ "name": "mystore-connector", "config": { "connector.class": "io.debezium.connector.mysql.MySqlConnector", "tasks.max": "1", "database.hostname": "mysql", "database.port": "3306", "database.user": "root", "database.password": "root", "database.server.id": "223345", "database.server.name": "mysql", "database.whitelist": "mystore", "database.history.kafka.bootstrap.servers": "kafka:9092", "database.history.kafka.topic": "dbhistory.mystore",
"transforms":"unwrap","transforms.unwrap.type":"io.debezium.transforms.UnwrapFromEnvelope","transforms.unwrap.drop.tombstones":"false","key.converter": "org.apache.kafka.connect.json.JsonConverter","key.converter.schemas.enable": "false","value.converter": "org.apache.kafka.connect.json.JsonConverter","value.converter.schemas.enable": "false","include.schema.changes": "false"} }'

kafka-console-consumer --bootstrap-server kafka:9092 --from-beginning --topic mysql.mycars.cars --property print.key=true --property key.separator="-"