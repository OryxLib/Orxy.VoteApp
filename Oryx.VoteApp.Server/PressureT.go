package main

import (
	"fmt"
	"net/http" 
	"sync"
	"time"
	"strings"
)

var errSum = 0
var count = 6000
var wg sync.WaitGroup
 
func main() {
	// wg.Add(count)
	  //postTest()
	postActual()
	time.Sleep(3)
	postActual2()
	time.Sleep(3)
	postActual3()
}
 
func getTest() {
	timeStart := time.Now()
	for index := 0; index < count; index++ {
		if index%1000 == 0 {
			// fmt.Println("run index ", index)
		}
		go func() {
			defer wg.Done()
			// fmt.Println("start :", index)
			resp, err := http.Get("http://hb.voteapp.oryxl.com/vote/getInfo?key=Linengneng")

			if err != nil {
				errSum++
			} else {
				if resp.StatusCode != 200 {
					errSum++
					fmt.Println(resp)
				} else {
					//fmt.Println(resp)
				}
			}
		}()
	}
	wg.Wait()
	fmt.Println("time : ", time.Since(timeStart))
	fmt.Println("err sum : ", errSum)
}
func postTest() {
	timeStart := time.Now()
	for index := 0; index < count; index++ {
		if index%1 == 0 {
			// fmt.Println("run index ", index)
		}

		go func() {
			defer wg.Done()
			// fmt.Println("start :", index)
			//resp, err := http.PostForm("http://hb.voteapp.oryxl.com/vote/Post",
			//resp, err := http.PostForm("https://voteapp.oryxl.com/vote/PostTest3?key=Linengneng",
				// 	url.Values{"VoteId": {"6"}, "VoteOptoin": {"测试"}, "UserId": {"1"}, "UserKey": {"test"}})
			//  resp, err := http.PostForm("http://localhost:41276/vote/Post",
			// 	url.Values{"VoteId": {"1"}, "VoteOption": {"这是谁"}, "UserId": {"1"}, "UserKey": {"test"}})
			// resp,err:=http.Post("https://voteapp.oryxl.com/vote/Post?key=Linengneng","application/json",strings.NewReader("{\"VoteLog\":{\"VoteId\":1,\"VoteOption\":\"这是哪?\",\"UserId\":1}}"))
			resp,err:=http.Post("http://hb.voteapp.oryxl.com/vote/Post?key=Linengneng","application/json",strings.NewReader("{\"VoteLog\":{\"VoteId\":1,\"VoteOption\":\"这是哪?\",\"UserId\":1}}"))
			// resp,err:=http.Post("http://ws.oryxl.com/vote/Post?key=Linengneng","application/json",strings.NewReader("{\"VoteLog\":{\"VoteId\":1,\"VoteOption\":\"这是哪?\",\"UserId\":1}}"))
			//  resp,err:=http.Post("http://localhost:19737/vote/PostTest3?key=Linengneng","application/json",strings.NewReader("{\"VoteLog\":{\"VoteId\":1,\"VoteOption\":\"这是哪?\",\"UserId\":1}}"))
			if err != nil {
				errSum++
				fmt.Println(err)
			} else {
				if resp.StatusCode != 200 {
					errSum++
					fmt.Println(resp)
				} else {
					//fmt.Println(result)
				}
			}
		}()
	}
	wg.Wait()
	fmt.Println("time : ", time.Since(timeStart))
	fmt.Println("err sum : ", errSum)
}

func postTest2(){
	resp,_:=http.Post("https://voteapp.oryxl.com/vote/Post?key=Linengneng","application/json",strings.NewReader("{\"VoteLog\":{\"VoteId\":1,\"VoteOption\":\"这是哪?\",\"UserId\":1}}"))
	fmt.Println(resp);
}

func postActual(){

	timeStart := time.Now()
	var psotData map[string]int
	psotData=make(map[string]int) 
	psotData["G01"]=345
	psotData["G02"]=441
	psotData["G03"]=224
	psotData["G04"]=532
	psotData["G05"]=133
	psotData["G06"]=102
	psotData["G07"]=213
	psotData["G08"]=99
	psotData["G09"]=119
	psotData["G10"]=271
    sum := 0
	for _,value := range psotData {
		sum+=value
	}
	wg.Add(sum)
	fmt.Println(sum)
	for	key, value := range psotData{ 
		for index := 0; index < value; index++{
			go func(_key string) {
				defer wg.Done() 
				resp,err:=http.Post("http://hb.voteapp.oryxl.com/vote/Post?key=Linengneng","application/json",strings.NewReader("{\"VoteLog\":{\"VoteId\":6,\"VoteOption\":\""+_key+"\",\"UserId\":1}}"))
				if err != nil {
					errSum++
					fmt.Println(err)
				} else {
					if resp.StatusCode != 200 {
						errSum++
						fmt.Println(resp)
					} else {
						//fmt.Println(result)
					}
				}
			}(key)
		}
	}
	wg.Wait()
	fmt.Println("time : ", time.Since(timeStart))
	fmt.Println("err sum : ", errSum)
}

func postActual2(){

	timeStart := time.Now()
	var psotData map[string]int
	psotData=make(map[string]int) 
	psotData["G01"]=345
	psotData["G02"]=441
	psotData["G03"]=224
	psotData["G04"]=532
	psotData["G05"]=133
	psotData["G06"]=3102
	psotData["G07"]=213
	psotData["G08"]=99
	psotData["G09"]=119
	psotData["G10"]=183
    sum := 0
	for _,value := range psotData {
		sum+=value
	}
	wg.Add(sum)
	fmt.Println(sum)
	for	key, value := range psotData{ 
		for index := 0; index < value; index++{
			go func(_key string) {
				defer wg.Done() 
				resp,err:=http.Post("http://hb.voteapp.oryxl.com/vote/Post?key=Linengneng","application/json",strings.NewReader("{\"VoteLog\":{\"VoteId\":6,\"VoteOption\":\""+_key+"\",\"UserId\":1}}"))
				if err != nil {
					errSum++
					fmt.Println(err)
				} else {
					if resp.StatusCode != 200 {
						errSum++
						fmt.Println(resp)
					} else {
						//fmt.Println(result)
					}
				}
			}(key)
		}
	}
	wg.Wait()
	fmt.Println("time : ", time.Since(timeStart))
	fmt.Println("err sum : ", errSum)
}

func postActual3(){

	timeStart := time.Now()
	var psotData map[string]int
	psotData=make(map[string]int) 
	psotData["G01"]=345
	psotData["G02"]=233
	psotData["G03"]=1125
	psotData["G04"]=147
	psotData["G05"]=267
	psotData["G06"]=321
	psotData["G07"]=231
	psotData["G08"]=245
	psotData["G09"]=233
	psotData["G10"]=163
    sum := 0
	for _,value := range psotData {
		sum+=value
	}
	wg.Add(sum)
	fmt.Println(sum)
	for	key, value := range psotData{ 
		for index := 0; index < value; index++{
			go func(_key string) {
				defer wg.Done() 
				resp,err:=http.Post("http://hb.voteapp.oryxl.com/vote/Post?key=Linengneng","application/json",strings.NewReader("{\"VoteLog\":{\"VoteId\":6,\"VoteOption\":\""+_key+"\",\"UserId\":1}}"))
				if err != nil {
					errSum++
					fmt.Println(err)
				} else {
					if resp.StatusCode != 200 {
						errSum++
						fmt.Println(resp)
					} else {
						//fmt.Println(result)
					}
				}
			}(key)
		}
	}
	wg.Wait()
	fmt.Println("time : ", time.Since(timeStart))
	fmt.Println("err sum : ", errSum)
}