var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

//oke!


//Alololololol
//Thiên Đây
//123/123/123/

//ALOOOOOOO
//Tân ăn cứt
//Tân hứa Tân vào mô, Tân ko vào??
//YOOOOOo

//dskfjskdfskfjskdfj



//Tan an nhieu cut

