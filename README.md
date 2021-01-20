# styxSignHelper

Локальный сервер, помогающий подписать строку с помощью ЭЦП Styx.


--Запрос
POST http://localhost:64321/SignString
Content-Type: application/json

{
"cert_sn":"30000058B1FAFF3D23A4CAB5880002000058B1",
"str_to_sign":"22.06.2020:123:00974:20208000012345678001:01088:20208000087654321002:100000"
}


--Ответ
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{"error":null,"result":{"cert_sn":"30000058B1FAFF3D23A4CAB5880002000058B1","signature":"","str_to_sign":"22.06.2020:123:00974:20208000012345678001:01088:20208000087654321002:100000"}}


--Порты TCP настраиваются в конфиг файле styxSignHelper.exe.config 

--Порт для REST API
    <add key="port" value="64321" />
    
--Порт для SOAP    
    <add key="soapport" value="64322" />
    
