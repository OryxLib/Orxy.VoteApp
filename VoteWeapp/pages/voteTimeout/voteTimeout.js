// pages/voteTimeout/voteTimeout.js
const app = getApp()
Page({

  /**
   * 页面的初始数据
   */
  data: {
    bgImg: app.globalData.bgImg,
    isConnected: false
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    this.setData({
      bgImg: app.globalData.bgImg
    })
  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  },
  startGetMsg: function () {
    var _this = this;
    if (app.globalData.waitNextVoteConnected) {
      _this.setData({
        isConnected: true
      })
      return;
    }
    wx.connectSocket({
      url: 'wss://voteapp.oryxl.com/ws?key=waitnextVote' || 'ws://localhost:19737/ws?key=waitnextVote' || ''
    })
    wx.onSocketOpen(function (res) {
      _this.setData({
        isConnected: true
      })
      app.globalData.waitNextVoteConnected = true;
      setInterval(function () {
        wx.sendSocketMessage({
          data: ['test data'],
        })
      }, 20000)
    })
    wx.onSocketMessage(function (res) {
      console.log('收到服务器内容：' + res.data)
      var resultObj = JSON.parse(res.data);
      app.globalData.endTimeStamp = resultObj.timestamp;

      switch (resultObj.cmd) {
        case 'nextVote':
          _this.gotoVote()
          // _this.setData({
          //   isEnable: true
          // })
          break;
      }
    })
    wx.onSocketError(function (res) {
      _this.setData({
        isConnected: false
      })
    })
    wx.onSocketClose(function (res) {
      _this.setData({
        isConnected: false
      })
    })
  },
  gotoVote: function () {
    wx.redirectTo({
      url: '/pages/vote/vote',
    })
  },
  goBack: function () {
    wx.redirectTo({
      url: '/pages/index/index',
    })
  },
  gotoRefresh: function () {
    // this.startGetMsg();
  }
})