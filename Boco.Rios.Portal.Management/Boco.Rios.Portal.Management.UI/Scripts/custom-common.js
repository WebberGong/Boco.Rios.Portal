function dateTime_UtcToLocal(dateTime) {
    if (dateTime == null || dateTime == "") {
        return dateTime;
    }
    var reg = /^\d{4}\-\d{2}\-\d{2}\s\d{2}\:\d{2}\:\d{2}$/;
    if (reg.test(dateTime)) {
        return dateTime;
    } else {
        try {
            var year = dateTime.getFullYear();
            var month = dateTime.getMonth() + 1;
            var day = dateTime.getDate();
            var hour = dateTime.getHours();
            var minute = dateTime.getMinutes();
            var second = dateTime.getSeconds();
            var result = year + "-" +
            padLeft(month, 2) + "-" +
            padLeft(day, 2) + " " +
            padLeft(hour, 2) + ":" +
            padLeft(minute, 2) + ":" +
            padLeft(second, 2);
            return result;
        } catch (e) {
            return dateTime;
        } 
    }
};
function dateTime_LocalToUtc(dateTime) {
    if (dateTime == null || dateTime == "") {
        return dateTime;
    }
    var reg = /^\d{4}\-\d{2}\-\d{2}\s\d{2}\:\d{2}\:\d{2}$/;
    if (reg.test(dateTime)) {
        try {
            var dateAndTime = dateTime.split(" ");
            var date = dateAndTime[0].split("-");
            var time = dateAndTime[1].split(":");
            var year = date[0];
            var month = date[1];
            var day = date[2];
            var hour = time[0];
            var minute = time[1];
            var second = time[2];
            return new Date(year, month, day, hour, minute, second);
        } catch (e) {
            return dateTime;
        } 
    } else {
        return dateTime;
    }
};
function padLeft(str, lenght) {
    if (str.length >= lenght)
        return str.substring(str.length - lenght);
    else
        return padLeft("0" + str, lenght);
};
function padRight(str, lenght) {
    if (str.length >= lenght)
        return str.substring(str.length - lenght);
    else
        return padRight(str + "0", lenght);
};
function htmlEncode(str) {
    var div = document.createElement("div");
    div.appendChild(document.createTextNode(str));
    return div.innerHTML;
};
function htmlDecode(str) {
    var div = document.createElement('div');
    div.innerHTML = str;
    return div.innerText || div.textContent;
};
function guid() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
};
function generateGUID() {
    var id = (guid() + guid() + "-" + guid() + "-" + guid() + "-" +
        guid() + "-" + guid() + guid() + guid()).toUpperCase();
    return id;
};