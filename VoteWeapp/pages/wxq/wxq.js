// const qiniuUploader = require("../../utils/qiniuUploader");
// //index.js

// // 初始化七牛相关参数
// function initQiniu() {
//   var options = {
//     region: 'ECN', // 华东区，生产环境应换成自己七牛帐户bucket的区域
//     uptokenURL: 'https://aime.getweapp.com/qiniu/uptoken', // 生产环境该地址应换成自己七牛帐户的token地址，具体配置请见server端
//     domain: 'https://oh39iobj6.qnssl.com/' // 生产环境该地址应换成自己七牛帐户对象存储的域名
//   };
//   qiniuUploader.init(options);
// }
// pages/wxq/wxq.js
const app = getApp()
Page({

  /**
   * 页面的初始数据
   */
  data: {
    pickarray: ["文本", "图片"],
    pickIndex: '0',
    uploadBucket: '',
    previewImgSrc: '',
    msgData: '',
    bgImg: 'https://mioto.milbit.com/voteapp/Goldbackground.png'
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {

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
  bindPickerChange: function (e) {
    this.setData({
      pickIndex: e.detail.value
    })
  },
  textInput: function (e) {
    this.setData({
      msgData: e.detail.value
    })
  },
  takePhoto: function () {
    var _this = this;
    wx.chooseImage({
      count: 1, // 默认9
      sizeType: ['original', 'compressed'], // 可以指定是原图还是压缩图，默认二者都有
      sourceType: ['album', 'camera'], // 可以指定来源是相册还是相机，默认二者都有
      success: function (res) {
        // 返回选定照片的本地文件路径列表，tempFilePath可以作为img标签的src属性显示图片
        var tempFilePaths = res.tempFilePaths
        _this.setData({
          previewImgSrc: tempFilePaths[0]
        })
      }
    })
  },
  postMsg: function (e) {
    var _this = this;
    switch (this.data.pickIndex) {
      case '0':
        this.sendTxt()
        break;
      case '1':
        this.sendImg()
        break;
    }

  },
  sendTxt: function () {
    var _msgData = {
      nickName: app.globalData.userInfo.nickName,
      avarta: app.globalData.userInfo.avatarUrl,
      msg: this.data.msgData,
      msgType: 'txt'
    };
    this.sendData(JSON.stringify(_msgData))
  },
  sendImg: function () {
    var _this = this;
    wx.request({
      url: app.globalData.baseUrl + '/game/GetToken?key=Linengneng',
      success: function (res) {
        console.log(res.data)
        _this.uploadImg(_this.data.previewImgSrc, res.data.token)
      },
      error: function () { }
    })
  },
  uploadImg: function (tmpPath, token) {
    var _this = this;
    wx.uploadFile({
      url: 'http://up-z1.qiniu.com', //仅为示例，非真实的接口地址
      filePath: tmpPath,
      name: 'file',
      formData: {
        'token': token,
        'key': new Date() - 0 + app.globalData.openId + '.jpg'
      },
      success: function (res) {
        var data = res.data
        console.log(res)
        var resObj = JSON.parse(res.data);
        var key = resObj.key;
        var imgUrl = 'https://mioto.milbit.com/' + key;

        var msgData = {
          nickName: app.globalData.userInfo.nickName,
          avarta: app.globalData.userInfo.avatarUrl,
          msg: imgUrl,
          msgType: 'img',
          date: new Date()
        };
        _this.sendData(JSON.stringify(msgData))
      }, error: function () { }
    })
  },
  sendData: function (sendingData) {
    wx.request({
      url: app.globalData.baseUrl + '/WxqFunc/PostCheckingData?key=Linengneng',
      method: 'post',
      data: {
        msg: sendingData
      },
      success: function (res) {
        console.log(res)
        wx.showToast({
          title: '发送成功',
        })
      },
      error: function () {

      }
    })
  },
  gobackVote: function () {
    wx.redirectTo({
      url: '/pages/index/index',
    })
  }
})