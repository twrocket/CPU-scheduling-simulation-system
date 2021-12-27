#include<stdio.h>
#include<stdlib.h>

qetype* read_file();//讀取檔案資料,並malloc空間來儲存
qetype* FCFS(qetype*,float*);//FCFS模擬
qetype* RR(qetype*,float*);//round robin模擬
qetype* SJF_non_preempty(qetype*,float*);//SJF non preempty模擬
qetype* SJF_preempty(qetype*,float*);//SJF preempty模擬
void print(qetype*,float);//用於print模擬結果
void sort_burst_time_and_add(queue**,queue**);//用於讓data queue的新資料加入ready queue 但加入會比較burst time大小,讓ready queue按照burst time大小,由小到大排列,SJF preempty模擬 和 SJF non preempty模擬 會使用此函式
void add_turnaround(queue*);//用於讓ready queue的每一個程序的turn around time加一,模擬turn around time隨時間的增加而慢慢增加

int main()
{
	qetype *data_queue,*waiting_queue;//data_queue儲存input檔案的資料,waiting_queue儲存模擬結果的資料
        int choise;//儲存user的選擇
	float CPU_utilization;//儲存模擬結果的cpu使用率
        printf("This is CPU scheduling simulater\n");
	(data_queue) = read_file(); //從檔案資料建立data queue
        printf("1.FCFS\n2.RR\n3.SJF non preempty\n4.SJF preempty\n");
        printf("input your choise:");
        scanf("%d",&choise);
        switch(choise){
                case 1:
                        waiting_queue = FCFS(data_queue,&CPU_utilization);//執行FCFS模擬,回傳結果給waiting_queue,並儲存cpu使用率數值
                        break;
                case 2:
                        waiting_queue = RR(data_queue,&CPU_utilization);//執行round robin模擬
                        break;
                case 3:
                        waiting_queue = SJF_non_preempty(data_queue,&CPU_utilization);//執行SJF non preempty模擬
                        break;
                case 4:
                        waiting_queue = SJF_preempty(data_queue,&CPU_utilization);//執行SJF preempty模擬
                        break;
        }
        print(waiting_queue,CPU_utilization);//列印模擬結果,和cpu使用率
	return 0;
}

qetype* FCFS(qetype *data_queue,float *CPU_utilization)
{
        int time=0,idle_time=0,finished_process_number=0;//time用於模擬時間的進行,會一直增加 idle_time紀錄閒置時間有多少 ,finished_process_number紀錄被完成的程序有幾筆
        qetype *waiting_queue,*ready_queue;//waiting_queue儲存模擬結果的資料,ready_queue第一筆程序模擬正在被cpu執行,其他筆則是正在等候
        waiting_queue = creat();//初始化
        ready_queue = creat();//初始化
        while((data_queue->front)!=NULL||ready_queue->front!=NULL)//當所有程序執行完後 跳出迴圈
        {
                while((data_queue->front)!=NULL&&(data_queue->front)->data.arrive_time==time)//當data_queue有東西,且time等於data_queue裡面程序的arrive_time,把該程序引進ready_queue
                {
                        enqueue(ready_queue,dequeue(data_queue));
                }
                if((ready_queue->front)!=NULL){//當ready_queue有東西,他的第一筆資料的burst_time減一,所有程序的turnaround加一
                        ((ready_queue->front)->data.burst_time)--;
                        add_turnaround(ready_queue->front);
                        if((ready_queue->front)->data.response_time==-1){//當ready_queue第一筆資料的response_time的數值等於初始化的值,把它的response_time等於現在的時間
                                (ready_queue->front)->data.response_time = time;}
                }
                else//當ready_queue沒東西,閒置時間+1
                        idle_time++;
                time++;//時間加一
                if((ready_queue->front)!=NULL&&(ready_queue->front)->data.burst_time==0){//當ready_queue有東西,且它的burst_time等於零代表該程序執行完畢
                        (ready_queue->front)->data.throughput = (float)(++finished_process_number)/(float)time;//計算當下的平均吞吐量
                        (ready_queue->front)->data.time = time;//紀錄該程序在何時完成
                        enqueue(waiting_queue,dequeue(ready_queue));}//把該程序模擬結果放到waiting_queue
        }
        *CPU_utilization = (float)(time-idle_time)/(float)time;
        return waiting_queue;
}
qetype* RR(qetype *data_queue,float *CPU_utilization)
{
        int time=0,idle_time=0,finished_process_number=0;
        int time_quantum,time_quantum_const;
        printf("please input time quantum(second)");
        scanf("%d",&time_quantum_const);//time quantum常數
        time_quantum = time_quantum_const;//time quantum變數
        qetype *waiting_queue,*ready_queue;
        waiting_queue = creat();
        ready_queue = creat();
        while((data_queue->front)!=NULL||ready_queue->front!=NULL)
        {
                while((data_queue->front)!=NULL&&(data_queue->front)->data.arrive_time==time)
                {
                        enqueue(ready_queue,dequeue(data_queue));
                }
                if((ready_queue->front)!=NULL){
                        ((ready_queue->front)->data.burst_time)--;
                        add_turnaround(ready_queue->front);
                        if((ready_queue->front)->data.response_time==-1){
                                (ready_queue->front)->data.response_time = time;}
                }
                else
                        idle_time++;
                time++;
                time_quantum--;
                if((ready_queue->front)!=NULL&&(ready_queue->front)->data.burst_time==0){//當ready_queue有東西,且它的burst_time等於零代表該程序執行完畢
                        (ready_queue->front)->data.throughput = (float)(++finished_process_number)/(float)time;
                        (ready_queue->front)->data.time = time;
                        enqueue(waiting_queue,dequeue(ready_queue));
                        time_quantum = time_quantum_const;}//重置time_quantum
                else if((ready_queue->front)!=NULL&&time_quantum==0){//代表該程序剩下的burst_time不為零,但time_quantum已減少至零
                        enqueue(ready_queue,dequeue(ready_queue));//把該程序從ready_queue的頭放到ready_queue的尾
                        time_quantum = time_quantum_const;}//重置time_quantum
        }
        *CPU_utilization = (float)(time-idle_time)/(float)time;
        return waiting_queue;
}
qetype* SJF_non_preempty(qetype *data_queue,float *CPU_utilization)
{
        int time=0,idle_time=0,finished_process_number=0;
        qetype *waiting_queue,*ready_queue;
        waiting_queue = creat();
        ready_queue = creat();
        while((data_queue->front)!=NULL||ready_queue->front!=NULL)
        {
                while((data_queue->front)!=NULL&&(data_queue->front)->data.arrive_time==time)
                {
                        if((ready_queue->front)==NULL)//當ready_queue沒東西就從data_queue 直接enqueue
                                enqueue(ready_queue,dequeue(data_queue));
                        else{//裡面有東西 要排序
                                sort_burst_time_and_add(&(data_queue->front),&((ready_queue->front)->Link));//不跟ready_queue第一個排序
                                ready_queue->count++;}//queue數量+1
                }
                if((ready_queue->front)!=NULL){
                        ((ready_queue->front)->data.burst_time)--;
                        add_turnaround(ready_queue->front);
                        if((ready_queue->front)->data.response_time==-1){
                                (ready_queue->front)->data.response_time = time;}
                }
                else
                        idle_time++;
                time++;
                if((ready_queue->front)!=NULL&&(ready_queue->front)->data.burst_time==0){
                        (ready_queue->front)->data.throughput = (float)(++finished_process_number)/(float)time;
                        (ready_queue->front)->data.time = time;
                        enqueue(waiting_queue,dequeue(ready_queue));}
        }
        *CPU_utilization = (float)(time-idle_time)/(float)time;
        return waiting_queue;
}
qetype* SJF_preempty(qetype *data_queue,float *CPU_utilization)
{
        int time=0,idle_time=0,finished_process_number=0;
        qetype *waiting_queue,*ready_queue;
        waiting_queue = creat();
        ready_queue = creat();
        while((data_queue->front)!=NULL||ready_queue->front!=NULL)
        {
                while((data_queue->front)!=NULL&&(data_queue->front)->data.arrive_time==time)
                {
                        sort_burst_time_and_add(&(data_queue->front),&(ready_queue->front));//會跟ready_queue第一個排序
                        ready_queue->count++;
                }
                if((ready_queue->front)!=NULL){
                        ((ready_queue->front)->data.burst_time)--;
                        add_turnaround(ready_queue->front);
                        if((ready_queue->front)->data.response_time==-1){
                                (ready_queue->front)->data.response_time = time;}
                }
                else
                        idle_time++;
                time++;
                if((ready_queue->front)!=NULL&&(ready_queue->front)->data.burst_time==0){
                        (ready_queue->front)->data.throughput = (float)(++finished_process_number)/(float)time;
                        (ready_queue->front)->data.time = time;
                        enqueue(waiting_queue,dequeue(ready_queue));}
        }
        *CPU_utilization = (float)(time-idle_time)/(float)time;
        return waiting_queue;
}
void print(qetype* qu,float CPU_utilization)
{
        queue *q;
        int i;
        q = qu->front;
        for(i=0;i<qu->count;i++)
        {
                printf("\nName:%s\n",q->data.name);
                printf("arrive_time:%d\t",q->data.arrive_time);
                printf("burst_time:%d\n",q->data.burst_time_const);
                printf("waiting_time:%d\t",(q->data.turnaround_time)-(q->data.burst_time_const));
                printf("turnaround_time:%d\t",q->data.turnaround_time);
                printf("response_time:%d\n",q->data.response_time);
                printf("throughput:%.2fprocess per second(at %d second)\n",q->data.throughput,q->data.time);
                q = q->Link;
        }
        printf("\nCPU_utilization:%.2fpercent\n",CPU_utilization*100);
}
void sort_burst_time_and_add(queue** q2,queue** q3)
{
        queue *q,*qa;
        q = *q2;
        qa = *q3;
        *q2 = (*q2)->Link;
        if(*q3==NULL){
                *q3 = q;
                (*q3)->Link = NULL;
                return;
        }
        if(((q->data).burst_time)<((qa->data).burst_time)){
                q->Link = *q3;
                *q3 = q;
                return;}
        else
                while((qa->Link)!=NULL)
                {
                        if(((q->data).burst_time)>=((qa->data).burst_time)&&((q->data).burst_time)<(((qa->Link)->data).burst_time)){
                                q->Link = qa->Link;
                                qa->Link = q;
                                return;}
                        qa = qa->Link;
                }
        qa->Link = q;
        q->Link = NULL;
}
void add_turnaround(queue* q3)
{
        while(q3!=NULL)
        {
                ((q3->data).turnaround_time)++;
                q3 = q3->Link;
        }
}
qetype* read_file()
{
        FILE* fp;
        char file[10];
        qetype *qe;
        dtype* d;
        qe = creat();
        printf("input file name:(test.txt)");
        scanf("%s",file);
        fp = fopen(file,"r");
        d = malloc(sizeof(dtype));
        while(fscanf(fp,"%s",d->name)!=EOF)
        {
                fscanf(fp,"%d",&(d->arrive_time));
                fscanf(fp,"%d",&(d->burst_time));
                d->turnaround_time = 0;
                d->burst_time_const = d->burst_time;
                d->response_time = -1;
                d->throughput = 0;
                enqueue(qe,*d);
                d = malloc(sizeof(dtype));
        }
        return qe;
}
