# Тестовое задание (для кандидата)

## Доменная область

Система, обрабатывающая данные о посетителях в кафе.

### Ассортимент кафе

В кафе должен быть определённый ассортимент, из которого могут выбирать клиенты. Система должна позволять добавлять позиции в него, редактировать их или убирать.

Каждая позиция в системе состоит из:
* Уникального идентификатора
* Уникального названия

### Совершение заказа

Если посетитель делает заказ, то данные об этом заказе поступают в систему для дальнейшей обработки.

При поступлении заказа, в систему должны попадать следующие данные:
* Имя, которым представился посетитель
* Время совершения заказа
* Способ оплаты: наличный, либо безналичный расчёт
* Список товаров из ассортимента, которые были заказаны

Каждый заказ должен иметь свой уникальный идентификатор, которые генерируется при добавлении в систему,
статус: "в работе", "выполнен", либо "отменён".
При первом появлении заказа в системе, его статус автоматически устанавливается в "в работе".

Каждая позиция в кафе имеет только своё уникальное название.

### Завершение заказов

Система должна позволять помечать заказы, как выполненные, либо как отменённые, меняя их статус соответственно.

Тут есть пара правил:
- Нельзя "отменить" выполненный заказ
- Нельзя "выполнить" отменённый заказ

### Чтение заказов

Система должна позволять получать список заказов за выбранный промежуток времени:
- Заказы "в работе"
- Выполненные заказы
- Отменённые заказы

Каждый "заказ" в данном случае должен содержать всю информацию, которая известна о нём в системе.

### Изменение заказов

Если заказ находится "в работе", то для него должна быть возможность изменить список товаров, которые
были заказаны по нему, т.е. в список заказанных товаров можно как добавить элементы, так и удалить их.

## Требования к системе

* Система должна быть реализована в виде RESTful API, которое предоставляет конечные точки для выполнения
  всех действий, которые описаны в доменной области. Использовать ASP.NET Core и .NET 8
* Нужно спроектировать БД по принципу Code First
* В качестве постоянного хранилища данных нужно использовать СУБД PostgreSQL
* Для взаимодействия с данными нужно использовать EF Core
* Система должна быть разделена на слои: API, доменный слой, слой данных
* Написаны Unit тесты, проверяющие работу функциональности, которая требуется в доменной области

Разработку системы нужно вести в GitHub, делая осмысленные коммиты.