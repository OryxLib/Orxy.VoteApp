// pages/voteWaiting.js
var app = getApp();
var timer = 0;
Page({

  /**
   * 页面的初始数据
   */
  data: {
    timeStampStart: 0,
    timeStamp: 0,
    timeStampDate: 0,
    isEnable: false,
    isConnected: false,
    bgImg: app.globalData.bgImg
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    console.log(options)
    this.setData({
      timeStampStart: new Date() + parseInt(options.timeStamp),
      timeStamp: parseInt(options.timeStamp)
    })
  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {
    // setTimeout(function () {
    //   wx.closeSocket()
    // }, 10000)
    this.setData({
      bgImg: app.globalData.bgImg
    })
    var _this = this;
    // timer = setInterval(function () {
    //   var timeshow = new Date().toLocaleString().replace(/^\.\d+Z$/g, '').replace(/\T/g, ' ')
    //   var timeToHoure = (_this.data.timeStamp - 60 * 60 * 1000)
    //   var timeToMiniutes = (_this.data.timeStamp - timeToHoure - 60 * 1000)
    //   var timeToSeconds = _this.data.timeStamp - timeToHoure - timeToMiniutes

    //   var t = _this.data.timeStamp
    //   var seconds = Math.floor((t / 1000) % 60)
    //   var minutes = Math.floor((t / 1000 / 60) % 60)
    //   var hours = Math.floor((t / (1000 * 60 * 60)) % 24)
    //   var days = Math.floor(t / (1000 * 60 * 60 * 24))

    //   if (_this.data.timeStamp > 0) {
    //     _this.setData({
    //       timeStamp: _this.data.timeStamp - 1000,
    //       timeStampStart: timeshow,
    //       timeStampDate: days + " 天 " + hours + " 时 " + minutes + " 分 " + seconds
    //     })
    //   } else {
    //     _this.setData({
    //       isEnable: true
    //     })
    //   }
    // }, 1000)
    _this.refresh();
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
  refresh: function () {
    var _this = this;
    app.gotoRefresh(function (isEnable) {
      if (isEnable) {
        _this.gotovote()
      }
      _this.setData({
        isEnable: isEnable,
        isConnected: app.globalData.waitNextVoteConnected
      })
    });
  },
  gotovote: function () {
    wx.redirectTo({
      url: '/pages/vote/vote',
    })
  },
})