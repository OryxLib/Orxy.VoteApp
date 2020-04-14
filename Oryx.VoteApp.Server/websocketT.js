const WebSocket = require('ws');

function createWS() {
    const ws = new WebSocket('ws://hb.voteapp.oryxl.com/ws?key=voteResult', {
        perMessageDeflate: false
    });

    ws.on('open', function open() {
        ws.send('something');
    });

    ws.on('message', function incoming(data) {
        // console.log(data);
    });

    ws.on('error',function(err){
       console.log(err);
    })
}

for (var index = 0; index < 2; index++) {
    process.nextTick(() => {
       createWS()
    }); 
}
