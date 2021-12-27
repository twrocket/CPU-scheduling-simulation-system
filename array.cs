#include<stdio.h>
#include<stdlib.h>

qetype* read_file();//讀取檔案資料,並malloc空間來儲存
qetype* FCFS(qetype*,float*);//FCFS模擬
qetype* RR(qetype*,float*);//round robin模擬
qetype* SJF_non_preempty(qetype*,float*);//SJF non preempty模擬
qetype* SJF_preempty(qetype*,float*);//SJF preempty模擬
void print(qetype*,float);//用於print模擬結果
void sort_burst_time_and_add(qetype*,qetype*);//用於讓data queue的新資料加入ready queue 但加入會比較burst time大小,讓ready queue按照burst time大小,由小到大排列,SJF preempty模擬 和 SJF non preempty模擬 會使用此函式
void add_turnaround(qetype*);//用於讓ready queue的每一個程序的turn around time加一,模擬turn around time隨時間的增加而慢慢增加

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
        print(waiting_queue,CPU_utilization); //列印模擬結果,和cpu使用率
}

qetype* FCFS(qetype *data_queue,float *CPU_utilization)
{
        int time=0,idle_time=0,finished_process_number=0;
        qetype *waiting_queue,*ready_queue;
        waiting_queue = creat();
        ready_queue = creat();
        while((data_queue->count)!=0||ready_queue->count!=0)
        {
                while((data_queue->count)!=0&&front(data_queue)->arrive_time==time)
                {
                        enqueue(ready_queue,dequeue(data_queue));
                }
                if((ready_queue->count)!=0){
                        front(ready_queue)->burst_time--;
                        add_turnaround(ready_queue);
                        if(front(ready_queue)->response_time==-1){
                                front(ready_queue)->response_time = time;}
                }
                else
                        idle_time++;
                time++;
                if((ready_queue->count)!=0&&front(ready_queue)->burst_time==0){
                        front(ready_queue)->throughput = (float)(++finished_process_number)/(float)time;
                        front(ready_queue)->time = time;
                        enqueue(waiting_queue,dequeue(ready_queue));}
        }
        *CPU_utilization = (float)(time-idle_time)/(float)time;
        return waiting_queue;
}
qetype* RR(qetype *data_queue,float *CPU_utilization)
{
        int time=0,idle_time=0,finished_process_number=0;
        int time_quantum,time_quantum_const;
        printf("please input time quantum(second)");
        scanf("%d",&time_quantum_const);
        time_quantum = time_quantum_const;
        qetype *waiting_queue,*ready_queue;
        waiting_queue = creat();
        ready_queue = creat();
        while((data_queue->count)!=0||ready_queue->count!=0)
        {
                while((data_queue->count)!=0&&front(data_queue)->arrive_time==time)
                {
                        enqueue(ready_queue,dequeue(data_queue));
                }
                if((ready_queue->count)!=0){
                        front(ready_queue)->burst_time--;
                        add_turnaround(ready_queue);
                        if(front(ready_queue)->response_time==-1){
                                front(ready_queue)->response_time = time;}
                }
                else
                        idle_time++;
                time++;
                time_quantum--;
                if((ready_queue->count)!=0&&front(ready_queue)->burst_time==0){
                        front(ready_queue)->throughput = (float)(++finished_process_number)/(float)time;
                        front(ready_queue)->time = time;
                        enqueue(waiting_queue,dequeue(ready_queue));
                        time_quantum = time_quantum_const;}
                else if((ready_queue->count)!=0&&time_quantum==0){
                        enqueue(ready_queue,dequeue(ready_queue));
                        time_quantum = time_quantum_const;}
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
        while((data_queue->count)!=0||ready_queue->count!=0)
        {
                while((data_queue->count)!=0&&front(data_queue)->arrive_time==time)
                {
                        if((ready_queue->count)==0)
                                enqueue(ready_queue,dequeue(data_queue));
                        else{
                                ready_queue->rear--;
                                ready_queue->array++;
                                ready_queue->count--;
                                sort_burst_time_and_add(ready_queue,data_queue);
                                ready_queue->rear++;
                                ready_queue->array--;
                                ready_queue->count++;
                                }
                }
                if((ready_queue->count)!=0){
                        front(ready_queue)->burst_time--;
                        add_turnaround(ready_queue);
                        if(front(ready_queue)->response_time==-1){
                                front(ready_queue)->response_time = time;}
                }
                else
                        idle_time++;
                time++;
                if((ready_queue->count)!=0&&front(ready_queue)->burst_time==0){
                        front(ready_queue)->throughput = (float)(++finished_process_number)/(float)time;
                        front(ready_queue)->time = time;
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
        while((data_queue->count)!=0||ready_queue->count!=0)
        {
                while((data_queue->count)!=0&&front(data_queue)->arrive_time==time)
                {
                        sort_burst_time_and_add(ready_queue,data_queue);
                }
                if((ready_queue->count)!=0){
                        front(ready_queue)->burst_time--;
                        add_turnaround(ready_queue);
                        if(front(ready_queue)->response_time==-1){
                                front(ready_queue)->response_time = time;}
                }
                else
                        idle_time++;
                time++;
                if((ready_queue->count)!=0&&front(ready_queue)->burst_time==0){
                        front(ready_queue)->throughput = (float)(++finished_process_number)/(float)time;
                        front(ready_queue)->time = time;
                        enqueue(waiting_queue,dequeue(ready_queue));}
        }
        *CPU_utilization = (float)(time-idle_time)/(float)time;
        return waiting_queue;
}
void print(qetype* qet,float CPU_utilization)
{
        int i;
        dtype* arr;
        arr = qet->array;
        for(i=0;i<qet->count;i++)
        {
                printf("\nName:%s\n",(arr+i)->name);
                printf("arrive_time:%d\t",(arr+i)->arrive_time);
                printf("burst_time:%d\n",(arr+i)->burst_time_const);
                printf("waiting_time:%d\t",((arr+i)->turnaround_time)-((arr+i)->burst_time_const));
                printf("turnaround_time:%d\t",(arr+i)->turnaround_time);
                printf("response_time:%d\n",(arr+i)->response_time);
                printf("throughput:%.2fprocess per second(at %d second)\n",(arr+i)->throughput,(arr+i)->time);
        }
        printf("\nCPU_utilization:%.2fpercent\n",CPU_utilization*100);
}
void sort_burst_time_and_add(qetype* ready_queue,qetype* data_queue)
{
        int i,j;
        dtype temp;
        temp = dequeue(data_queue);
        if(ready_queue->count==0){
                enqueue(ready_queue,temp);
                //enqueue(ready_queue,dequeue(data_queue));
                return;
        }
        if((temp.burst_time)<(front(ready_queue)->burst_time)){
                for(i=ready_queue->rear;i>=ready_queue->front;i--){
                        *(ready_queue->array+i+1) = *(ready_queue->array+i);
                }
                ready_queue->rear++;
                *(front(ready_queue)) = temp;
                ready_queue->count++;
                return;}
        else
                for(i=ready_queue->front;i<ready_queue->rear;i++)
                {
                        if(((ready_queue->array+i)->burst_time)<=(temp.burst_time)&&(temp.burst_time)<(((ready_queue->array+i+1)->burst_time))){
                                for(j=ready_queue->rear;j>=i+1;j--){
                                        *(ready_queue->array+j+1) = *(ready_queue->array+j);}
                                ready_queue->rear++;
                                *(ready_queue->array+i+1) = temp;
                                ready_queue->count++;
                                return;}
                }
        ready_queue->rear++;
        *(ready_queue->array+(ready_queue->rear)) = temp;
        ready_queue->count++;
}
void add_turnaround(qetype* qet)
{
        int i;
        for(i=qet->front;i<=qet->rear;i++)
        {
                ((qet->array+i)->turnaround_time)++;
        }
}
qetype* read_file()
{
        FILE* fp;
        char file[10];
        qetype *qe;
        dtype d;
        qe = creat();
        printf("input file name:(test.txt)");
        scanf("%s",file);
        fp = fopen(file,"r");
        while(fscanf(fp,"%s",d.name)!=EOF)
        {
                fscanf(fp,"%d",&(d.arrive_time));
                fscanf(fp,"%d",&(d.burst_time));
                d.turnaround_time = 0;
                d.burst_time_const = d.burst_time;
                d.response_time = -1;
                d.throughput = 0;
                enqueue(qe,d);
        }
        return qe;
}