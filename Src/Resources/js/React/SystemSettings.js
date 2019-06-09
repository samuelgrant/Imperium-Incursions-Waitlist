import React, { Component } from 'react';
import { render } from 'react-dom';
import Card from './Components/Card';

const baseUri = "/admin/settings";

export default class SystemSettings extends Component {
    constructor(props) {
        super(props);

        this.shipInput = React.createRef();
    }

    componentDidMount() {
        this.getShips();
    }

    //Ajax call to API to get data
    getShips() {
        $.ajax({
            type: 'get',
            url: `${baseUri}/data`,
        }).done((data) => {
            this.setState({
                settings: data
            });
        }).fail((err) => {
            console.error(`React/SystemSettings {SystemSettings@getShips} - Error getting the current ship queues`, err.responseText);
        })
    }

    NewQueueAssignment(queue_id) {
        $.ajax({
            type: 'post',
            url: `${baseUri}/ships`,
            data: { queue_id: queue_id, ship_name: this.shipInput.current.value}
        }).done(() => {
            this.getShips();
        }).fail((err) => {
            console.error(`React/SystemSettings {SystemSettings@NewQueueAssignment} - Error setting a new queue assignment`, err.responseText);
        });
    }

    updateShip(e) {
        $.ajax({
            type: 'put',
            url: `${baseUri}/ships`,
            data: { ship_id: e.target.value.split(',')[0], queue_id: e.target.value.split(',')[1]}
        }).done(() => {
            this.getShips();
        }).fail((err) => {
            console.error(`React/SystemSettings {SystemSettings@updateShip} - Error getting the current ship queues`, err.responseText);
        })
    }

    render() {
        let shipQueue_Row;
        if (this.state.settings && this.state.settings.hull) {
            shipQueue_Row = this.state.settings.hull.map((ship) => {
                let options;
                options = this.state.settings.queues.map((queue, key) => {
                    let isSelected = (ship.queue == key) ? true : false;
                    return <option value={`${ship.id},${key}`} selected={isSelected}>{queue}</option>
                })

                return (
                    <tr>
                        <td><img src={`https://image.eveonline.com/Render/${ship.id}_32.png`} alt="Ship Render" /></td>
                        <td>{ship.name}</td>
                        <td>
                            <select className="form-control" onChange={this.updateShip.bind(this)}>
                                {options}
                            </select>
                        </td>
                    </tr>
                )
            });
        }

        let shipQueue_DropdownOptions;
        if (this.state.settings && this.state.settings.queues) {
            shipQueue_DropdownOptions = this.state.settings.queues.map((queue, key) => {
                return <a className="dropdown-item" role="presentation" onClick={this.NewQueueAssignment.bind(this, key)}>{queue}</a>
            });
        }

        return (
            <div className="container">
                <div className="row">
                    <div className="col-lg-4 col-md-6 col-sm-12">
                        <Card heading="Ship Queues">
                            <p>This list defines what queues a ship goes into, ships that are not on this list will go into DPS. To remove a ship add it to the queue "none"</p>
                            <div className="input-group">
                                <input id="ship_search" type="text" className="form-control" placeholder="Erebus" ref={this.shipInput}/>
                                <div className="input-group-prepend">
                                    <div className="dropdown btn-group" role="group">
                                        <button className="btn btn-dark dropdown-toggle" data-toggle="dropdown" aria-expanded="false">Add with Role </button>
                                        <div className="dropdown-menu" role="menu">
                                            {shipQueue_DropdownOptions}
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="table-responsive">
                                <table class="table">
                                    <thead>
                                        <tr>
                                            <th></th>
                                            <th>Type</th>
                                            <th>Queue</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {shipQueue_Row}
                                    </tbody>
                                </table>
                            </div>
                        </Card>
                    </div>
                </div>
            </div>
        );
    }
}

if (document.getElementById('systemSettings'))
    render(<SystemSettings />, document.getElementById('systemSettings'));