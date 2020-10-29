//<script>
        setInterval(function () {
            // set whatever future date / time you want here, together with
            // your timezone setting...
            //var future = new Date("Feb 04 2016 2:00:00 GMT/UTC+05:30");
            var future = new Date("02/13/2016 12:27:00 PM GMT+05:30");
            var now = new Date();
            var difference = Math.floor((future - now) / 1000);

            var seconds = fixIntegers(difference % 60);
            difference = Math.floor(difference / 60);

            var minutes = fixIntegers(difference % 60);
            difference = Math.floor(difference / 60);

            var hours = difference;
            //var hours = fixIntegers(difference % 24);
            //difference = Math.floor(difference / 24);

            //var days = difference;

            $("#seconds").text(seconds + "");
            $("#minutes").text(minutes + "");
            $("#hours").text(hours + "");
            // $("#days").text(days + "d");
        }, 1000);

        function fixIntegers(integer) {
            if (integer < 0)
                integer = 0;
            if (integer < 10)
                return "0" + integer;
            return "" + integer;
        }
    //</script>