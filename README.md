# Projeto

- Debezium para escutar mudanças no MySql
- Kafka para criar um tópico que armazena essas alterações
- Service Worker que ficará rodando em background consumindo o tópico e atualizando o cache
- Web API que nos retorna o cache sempre atualizado.

## Instalação

Use o docker-compose para subir os serviços necessários

- Mysql que é nosso Banco de dados.
- Zookeeper é responsável por armazenar nossas chaves e valores
- Kafka para que possamos criar a fila
- Connector para ouvirmos as alterações na base.

```bash
docker-compose up
```

Carregado informações no MySql

```bash
docker-compose exec mysql bash -c "mysql -u root -p\$MYSQL_ROOT_PASSWORD"
```

Password = `root`

```bash
create database cardb;
```

```bash
use cardb;
```

```bash
create table cars
(id int unsigned auto_increment primary key,
name varchar(50),
color varchar(15),
price int,
creation_time datetime default current_timestamp,
modification_time datetime on update current_timestamp);
```

```bash
insert into cars(name, color, price) values("Fusca", "Amarelo", 50.000);
update cars set price = 10.000 where name = "Fusca";
delete from cars where id = 1;
```

Configurando o nosso conector `carddb-connector` para pegarmos as alterações na base de dados:

Abra uma nova aba no terminal e execute:

```bash
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
```

Para verificarmos se nosso conector está enviando para a fila, execute:

```bash
docker-compose exec kafka bash
```

```bash
kafka-console-consumer --bootstrap-server kafka:9092 --from-beginning
```

Obs: Note que usamos o `--from-beginning` para trazer as mensagem da fila desde o começo.

## Testes

Temos dois testes, um para retorno vazio caso o chache não estaja preenchido e outro com o cache já preenchido.

![alt text](https://raw.githubusercontent.com/vinicostaa/kafka-mysql/master/test.png)
